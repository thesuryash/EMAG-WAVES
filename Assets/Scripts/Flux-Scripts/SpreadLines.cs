using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SpreadLines : MonoBehaviour
{
    [SerializeField] private LineRenderer baseLine;
    [SerializeField] private GameObject baseArrow;
    [SerializeField] private Transform parent;
    [SerializeField] private GameObject rendererParent;
    [SerializeField] private Slider ElectricField;
    [SerializeField] private GameObject ElectricFieldArrow;
    [SerializeField] private GameObject StaticArrowParent;
/*    [SerializeField] private Slider Length;
    [SerializeField] private Slider Width;*/
    private Vector3 pos;
    private Queue<LineRenderer> linePool = new Queue<LineRenderer>();
    private List<LineRenderer> activeLineRenderers = new List<LineRenderer>();
    private float maxLines;

    private void Awake()
    {
        pos = baseLine.transform.position;
        maxLines = 1000 / Mathf.Sqrt(5000); // Adjusted for an initial size of the pool
        InitializeLinePool();

        // Add listeners to sliders
        ElectricField.onValueChanged.AddListener(delegate { OnSliderChange(); });

    }

    private void InitializeLinePool()
    {
        for (int i = 0; i < maxLines; i++)
        {
            LineRenderer lr = Instantiate(baseLine, parent);
            lr.gameObject.SetActive(false); // Initially deactivate
            linePool.Enqueue(lr); // Add to the queue
        }
    }

    private LineRenderer GetLineRendererFromPool()
    {
        if (linePool.Count > 0)
        {
            LineRenderer lr = linePool.Dequeue();
            lr.gameObject.SetActive(true);
            activeLineRenderers.Add(lr);
            return lr;
        }
        else
        {
            LineRenderer lr = Instantiate(baseLine, parent);
            lr.gameObject.SetActive(true);
            activeLineRenderers.Add(lr);
            return lr;
        }
    }

    private void ReturnLineRendererToPool(LineRenderer lr)
    {
        lr.gameObject.SetActive(false);
        linePool.Enqueue(lr);
        activeLineRenderers.Remove(lr);
    }

    private void OnSliderChange()
    {
        float step = StepSizeByEField();
        SpreadFieldLines(step);

        if(ElectricField.value == 0)
        {
            rendererParent.SetActive(false);
            ElectricFieldArrow.SetActive(false);
            baseArrow.SetActive(false);
            //StaticArrowParent.SetActive(false);
            baseLine.enabled = false;
        }
        else
        {
            rendererParent.SetActive(true);
            ElectricFieldArrow.SetActive(true);
            baseArrow.SetActive(true);
            //StaticArrowParent.SetActive(true);
            baseLine.enabled = true;
        }
    }

    private float StepSizeByEField()
    {
        if (ElectricField.value == 0)
        {
            baseLine.enabled = false;
            return float.MaxValue; // No lines when there's no electric field.
        }
        baseLine.enabled = true;

        // Use an inverse function to decrease step size as ElectricField.value increases
        //float stepSize = 25 / Mathf.Sqrt(ElectricField.value);
        float stepSize = 5 / Mathf.Sqrt(ElectricField.value);


        // Clamp the step size to maintain a practical range
        stepSize = Mathf.Clamp(stepSize, 1.0f, 51.0f); // Ensure step size stays between 2 and 20

        return stepSize;
    }





    private void SpreadFieldLines(float stepSize)
    {
        ResetAndPoolAllLines();

        Vector3 startPosX = new Vector3(pos.x - stepSize * Mathf.Floor(10f/stepSize), pos.y, pos.z);
        Vector3 endPosX = new Vector3(pos.x + stepSize * Mathf.Floor(10f / stepSize), pos.y, pos.z);
        Vector3 stepX = new Vector3(stepSize, 0, 0);

        for (Vector3 i = startPosX; i.x <= endPosX.x; i += stepX)
        {
            Vector3 startPosY = new Vector3(i.x, pos.y - stepSize * Mathf.Floor(10f / stepSize), pos.z);
            Vector3 endPosY = new Vector3(i.x, pos.y + stepSize * Mathf.Floor(10f / stepSize), pos.z);
            Vector3 stepY = new Vector3(0, stepSize, 0);

            for (Vector3 j = startPosY; j.y <= endPosY.y; j += stepY)
            {
                LineRenderer lr = GetLineRendererFromPool();
                lr.transform.position = new Vector3(i.x, j.y, pos.z);
                /*lr.startColor = new Color(0.537f, 0.812f, 0.941f, 1);
                lr.endColor = new Color(0.537f, 0.812f, 0.941f, 1);*/
            }
        }
    }

    private void ResetAndPoolAllLines()
    {
        foreach (LineRenderer lr in new List<LineRenderer>(activeLineRenderers))
        {
            ReturnLineRendererToPool(lr);
        }
    }

    private void OnDestroy()
    {
        // Remove listeners when the object is destroyed
        ElectricField.onValueChanged.RemoveAllListeners();

    }
}



