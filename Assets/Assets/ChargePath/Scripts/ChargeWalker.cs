using System.Collections.Generic;
using UnityEngine;

public class CurrentVisualizer : MonoBehaviour
{
    [Header("Connections")]
    [SerializeField] private CircuitLoop circuitPath;
    [SerializeField] private GameObject chargePrefab;

    [Header("Live Data (Read Only)")]
    public float currentAmps = 0f;

    [Header("Visual Settings")]
    [Tooltip("How many charges to spawn.")]
    [Range(1, 100)] public int chargeCount = 20;

    [Tooltip("Speed multiplier. Adjust to make movement look realistic.")]
    public float currentSensitivity = 0.5f;

    // Internal lists
    private List<Transform> _spawnedCharges = new List<Transform>();
    private float[] _chargeProgress; // 0.0 to 1.0 value for each charge
    private float _totalPathLength;

    private void Start()
    {
        if (circuitPath == null) circuitPath = GetComponent<CircuitLoop>();

        // 1. Calculate the total length of the wire (for constant speed)
        RecalculatePathLength();

        // 2. Spawn and Space the charges
        SpawnCharges();
    }

    private void Update()
    {
        if (_spawnedCharges.Count == 0) return;
        if (Mathf.Approximately(currentAmps, 0f)) return;

        MoveCharges();

        //Debug.Log($"Visualizer sees: {currentAmps}");
    }

    // --- SETUP LOGIC ---

    public void RecalculatePathLength()
    {
        _totalPathLength = 0f;
        List<Vector3> pts = circuitPath.waypoints;

        for (int i = 0; i < pts.Count; i++)
        {
            Vector3 p1 = pts[i];
            Vector3 p2 = pts[(i + 1) % pts.Count]; // Wrap to start
            _totalPathLength += Vector3.Distance(p1, p2);
        }
    }

    private void SpawnCharges()
    {
        // Clear old ones if any
        foreach (var t in _spawnedCharges) Destroy(t.gameObject);
        _spawnedCharges.Clear();

        _chargeProgress = new float[chargeCount];

        for (int i = 0; i < chargeCount; i++)
        {
            GameObject obj = Instantiate(chargePrefab, transform); // Child of Circuit
            _spawnedCharges.Add(obj.transform);

            // Distribute evenly from 0.0 to 1.0
            _chargeProgress[i] = (float)i / chargeCount;

            // Initial Position Update
            UpdateSingleChargePosition(i);
        }
    }

    // --- MOVEMENT LOGIC ---

    private void MoveCharges()
    {
        // Calculate how much "percentage of the path" we move this frame
        // Distance = Velocity * Time
        // Percentage = Distance / TotalLength
        float moveDelta = (currentAmps * currentSensitivity * Time.deltaTime) / _totalPathLength;

        for (int i = 0; i < chargeCount; i++)
        {
            // Update progress
            _chargeProgress[i] += moveDelta;

            // Handle wrapping (0.0 to 1.0) correctly for negative numbers
            if (_chargeProgress[i] > 1f) _chargeProgress[i] -= 1f;
            else if (_chargeProgress[i] < 0f) _chargeProgress[i] += 1f;

            UpdateSingleChargePosition(i);
        }
    }

    private void UpdateSingleChargePosition(int index)
    {
        // 1. Get the local position on the wire
        Vector3 localPos = GetPointAtUnitProgress(_chargeProgress[index]);

        // 2. Convert to World Space (handles parent rotation/scale)
        // Note: transform.TransformPoint converts Local -> World
        _spawnedCharges[index].position = transform.TransformPoint(localPos);
    }

    // --- INTERPOLATION MATH ---
    // Converts a 0.0-1.0 value into a specific position on the polygon/loop
    private Vector3 GetPointAtUnitProgress(float t)
    {
        // Map t (0..1) to actual length
        float targetDist = t * _totalPathLength;
        float currentDist = 0f;
        List<Vector3> pts = circuitPath.waypoints;

        // Iterate through segments to find which one we are on
        for (int i = 0; i < pts.Count; i++)
        {
            Vector3 p1 = pts[i];
            Vector3 p2 = pts[(i + 1) % pts.Count];
            float segmentLen = Vector3.Distance(p1, p2);

            if (currentDist + segmentLen >= targetDist)
            {
                // We are inside this segment
                float remaining = targetDist - currentDist;
                float segmentT = remaining / segmentLen; // 0..1 within this segment
                return Vector3.Lerp(p1, p2, segmentT);
            }

            currentDist += segmentLen;
        }

        return pts[0]; // Fallback
    }
}