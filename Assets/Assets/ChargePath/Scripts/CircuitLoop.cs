using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(LineRenderer))]
public class CircuitLoop : MonoBehaviour
{
    public enum PlaneOrientation { XZ_Floor, XY_Front, YZ_Side }

    [HideInInspector]
    public List<Vector3> waypoints = new List<Vector3>();

    [Header("General Settings")]
    public PlaneOrientation orientation = PlaneOrientation.XZ_Floor;
    [Range(0f, 360f)] public float shapeRotation = 0f; // NEW: Rotation within the plane
    public bool isClosedLoop = true;

    [Header("Ellipse Settings")]
    public float radiusPrimary = 5f;
    public float radiusSecondary = 3f;
    [Range(3, 100)] public int curveResolution = 40;

    [Header("Polygon Settings")]
    [Min(3)] public int numberOfSides = 5;
    public float sideLength = 3f;

    private LineRenderer _lr;

    private void OnValidate()
    {
        // Optional: Uncomment the next line if you want the shape to update 
        // INSTANTLY as you drag the sliders (can be heavy if point count is huge)
        // GeneratePolygon(); 

        UpdateLineRenderer();
    }

    public void UpdateLineRenderer()
    {
        if (_lr == null) _lr = GetComponent<LineRenderer>();
        _lr.useWorldSpace = false; // Local space logic

        if (waypoints == null || waypoints.Count == 0) return;

        int count = isClosedLoop ? waypoints.Count + 1 : waypoints.Count;
        _lr.positionCount = count;

        for (int i = 0; i < waypoints.Count; i++)
        {
            _lr.SetPosition(i, waypoints[i]);
        }

        if (isClosedLoop && waypoints.Count > 0)
        {
            _lr.SetPosition(waypoints.Count, waypoints[0]);
        }
    }

    // --- MATH HELPERS ---

    private Vector3 GetPointOnPlane(float u, float v)
    {
        // 1. Apply the Shape Rotation (2D Rotation Formula)
        //    x' = x cos θ - y sin θ
        //    y' = x sin θ + y cos θ
        float rad = shapeRotation * Mathf.Deg2Rad;
        float uRot = u * Mathf.Cos(rad) - v * Mathf.Sin(rad);
        float vRot = u * Mathf.Sin(rad) + v * Mathf.Cos(rad);

        // 2. Map to 3D Plane
        switch (orientation)
        {
            case PlaneOrientation.XY_Front: return new Vector3(uRot, vRot, 0);
            case PlaneOrientation.YZ_Side: return new Vector3(0, vRot, uRot);
            case PlaneOrientation.XZ_Floor: default: return new Vector3(uRot, 0, vRot);
        }
    }

    // --- GENERATORS ---

    public void GenerateEllipse()
    {
        waypoints.Clear();
        for (int i = 0; i < curveResolution; i++)
        {
            float angle = i * Mathf.PI * 2 / curveResolution;

            // Raw 2D coords
            float u = Mathf.Cos(angle) * radiusPrimary;
            float v = Mathf.Sin(angle) * radiusSecondary;

            waypoints.Add(GetPointOnPlane(u, v));
        }
        UpdateLineRenderer();
    }

    public void GeneratePolygon()
    {
        waypoints.Clear();
        float anglePerSide = 360f / numberOfSides;
        float halfAngleRad = (anglePerSide / 2f) * Mathf.Deg2Rad;
        float radius = sideLength / (2f * Mathf.Sin(halfAngleRad));

        for (int i = 0; i < numberOfSides; i++)
        {
            float angleDeg = i * anglePerSide;
            float angleRad = angleDeg * Mathf.Deg2Rad;

            // Raw 2D coords
            float u = Mathf.Cos(angleRad) * radius;
            float v = Mathf.Sin(angleRad) * radius;

            waypoints.Add(GetPointOnPlane(u, v));
        }
        UpdateLineRenderer();
    }
}