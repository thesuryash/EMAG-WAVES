using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SpreadLines : MonoBehaviour
{
<<<<<<< HEAD
    [SerializeField] private LineRenderer baseLine;
    [SerializeField] private GameObject baseArrow;
    [SerializeField] private Transform parent;
    [SerializeField] private GameObject rendererParent;
    [SerializeField] private Slider ElectricField;
    [SerializeField] private GameObject ElectricFieldArrow;
    [SerializeField] private GameObject StaticArrowParent;
/*  [SerializeField] private Slider Length;
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





=======
    [Header("References")]
    [SerializeField] private LineRenderer linePrefab;
    [SerializeField] private Transform poolContainer;      // Parent for pooled line objects
    [SerializeField] private GameObject visualRoot;        // Root to enable/disable visuals (optional)

    [Header("UI")]
    [SerializeField] private Slider electricFieldSlider;

    [Header("Toggles")]
    [SerializeField] private List<GameObject> toggleWithField = new(); // arrows, parents, etc.
    [SerializeField] private bool alsoTogglePrefabLineRenderer = true; // if you want linePrefab.enabled toggled too

    [Header("Grid Settings")]
    [SerializeField] private float halfExtent = 10f;       // was hardcoded 10f
    [SerializeField] private float stepNumerator = 5f;     // was 5 / sqrt(E)
    [SerializeField] private Vector2 stepClamp = new Vector2(1f, 51f);

    [Header("Pooling")]
    [SerializeField] private int initialPoolSize = 200;    // sensible default, not magic

    private readonly Queue<LineRenderer> pool = new();
    private readonly List<LineRenderer> active = new();

    private void Awake()
    {
        ValidateRefs();
        InitializePool(initialPoolSize);

        electricFieldSlider.onValueChanged.AddListener(OnElectricFieldChanged);
        OnElectricFieldChanged(electricFieldSlider.value); // apply initial state
    }

    private void ValidateRefs()
    {
        if (!linePrefab) Debug.LogError($"{nameof(SpreadLines)}: Missing linePrefab.", this);
        if (!poolContainer) Debug.LogError($"{nameof(SpreadLines)}: Missing poolContainer.", this);
        if (!electricFieldSlider) Debug.LogError($"{nameof(SpreadLines)}: Missing electricFieldSlider.", this);
    }

    private void InitializePool(int count)
    {
        for (int i = 0; i < count; i++)
        {
            var lr = Instantiate(linePrefab, poolContainer);
            lr.gameObject.SetActive(false);
            pool.Enqueue(lr);
        }
    }

    private void OnElectricFieldChanged(float value)
    {
        bool enabled = value > 0f;

        if (visualRoot) visualRoot.SetActive(enabled);
        for (int i = 0; i < toggleWithField.Count; i++)
        {
            if (toggleWithField[i]) toggleWithField[i].SetActive(enabled);
        }

        if (alsoTogglePrefabLineRenderer && linePrefab)
            linePrefab.enabled = enabled;

        if (!enabled)
        {
            ResetAndPoolAllLines();
            return;
        }

        float step = StepSizeByEField(value);
        SpreadFieldLines(step);
    }

    private float StepSizeByEField(float eField)
    {
        // step = stepNumerator / sqrt(E)
        float step = stepNumerator / Mathf.Sqrt(eField);
        return Mathf.Clamp(step, stepClamp.x, stepClamp.y);
    }

>>>>>>> c5de63b56bf9714503326fda9cc4eb4efcb49210
    private void SpreadFieldLines(float stepSize)
    {
        ResetAndPoolAllLines();

<<<<<<< HEAD
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
=======
        Vector3 origin = linePrefab.transform.position; // don’t cache if it could move

        float snapped = stepSize * Mathf.Floor(halfExtent / stepSize);
        float minX = origin.x - snapped;
        float maxX = origin.x + snapped;
        float minY = origin.y - snapped;
        float maxY = origin.y + snapped;

        for (float x = minX; x <= maxX; x += stepSize)
        {
            for (float y = minY; y <= maxY; y += stepSize)
            {
                LineRenderer lr = GetFromPool();
                lr.transform.position = new Vector3(x, y, origin.z);
>>>>>>> c5de63b56bf9714503326fda9cc4eb4efcb49210
            }
        }
    }

<<<<<<< HEAD
    private void ResetAndPoolAllLines()
    {
        foreach (LineRenderer lr in new List<LineRenderer>(activeLineRenderers))
        {
            ReturnLineRendererToPool(lr);
        }
=======
    private LineRenderer GetFromPool()
    {
        LineRenderer lr = pool.Count > 0 ? pool.Dequeue() : Instantiate(linePrefab, poolContainer);
        lr.gameObject.SetActive(true);
        active.Add(lr);
        return lr;
    }

    private void ReturnToPool(LineRenderer lr)
    {
        lr.gameObject.SetActive(false);
        pool.Enqueue(lr);
    }

    private void ResetAndPoolAllLines()
    {
        for (int i = active.Count - 1; i >= 0; i--)
        {
            ReturnToPool(active[i]);
        }
        active.Clear();
>>>>>>> c5de63b56bf9714503326fda9cc4eb4efcb49210
    }

    private void OnDestroy()
    {
<<<<<<< HEAD
        // Remove listeners when the object is destroyed
        ElectricField.onValueChanged.RemoveAllListeners();

    }
}



=======
        if (electricFieldSlider)
            electricFieldSlider.onValueChanged.RemoveListener(OnElectricFieldChanged);
    }
}
>>>>>>> c5de63b56bf9714503326fda9cc4eb4efcb49210
