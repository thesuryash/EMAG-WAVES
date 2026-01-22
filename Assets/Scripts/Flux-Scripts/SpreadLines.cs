using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SpreadLines : MonoBehaviour
{
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

    private void SpreadFieldLines(float stepSize)
    {
        ResetAndPoolAllLines();

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
            }
        }
    }

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
    }

    private void OnDestroy()
    {
        if (electricFieldSlider)
            electricFieldSlider.onValueChanged.RemoveListener(OnElectricFieldChanged);
    }
}
