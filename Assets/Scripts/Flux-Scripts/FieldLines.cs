using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[RequireComponent(typeof(LineRenderer))]
public class FieldLines : MonoBehaviour
{
    [SerializeField] public LineRenderer fieldLineBase;

    public Transform target; // The target object around which the field lines will be generated
    [SerializeField] private int numberOfLines = 10; // Number of field lines on each side
    [SerializeField] private float lineLength = 10.0f; // Length of each field line
    [SerializeField] private float lineWidth = 0.1f; // Width of the field lines
    [SerializeField] private float spacing = 1.0f; // Distance between the field lines

    private LineRenderer[] lineRenderers;
    private int speed = 0;
    private Vector3Int basePosition = new Vector3Int (0,0,0);

    void Start()
    {
        if (target == null)
        {
            Debug.LogError("Target not set for ElectricFieldLines script.");
            return;
        }

        lineRenderers = new LineRenderer[numberOfLines * 4]; // For four sides
        for (int i = 0; i < numberOfLines; i++)
        {
            // Top side
            CreateLineRenderer(i, new Vector3(-lineLength / 2, i * spacing, 0), new Vector3(lineLength / 2, i * spacing, 0));
            // Bottom side
            CreateLineRenderer(i + numberOfLines, new Vector3(-lineLength / 2, -i * spacing, 0), new Vector3(lineLength / 2, -i * spacing, 0));
            // Left side
            CreateLineRenderer(i + numberOfLines * 2, new Vector3(-i * spacing, -lineLength / 2, 0), new Vector3(-i * spacing, lineLength / 2, 0));
            // Right side
            CreateLineRenderer(i + numberOfLines * 3, new Vector3(i * spacing, -lineLength / 2, 0), new Vector3(i * spacing, lineLength / 2, 0));
        }
    }

    private void CreateLineRenderer(int index, Vector3 localStart, Vector3 localEnd)
    {   

        GameObject lineObj = new GameObject("Line" + index);
        lineObj.transform.SetParent(transform);
        LineRenderer lineRenderer = lineObj.AddComponent<LineRenderer>();

        lineRenderer.startWidth = lineWidth;
        lineRenderer.endWidth = lineWidth;
        lineRenderer.useWorldSpace = true;

        Vector3 startPosition = target.position + localStart;
        Vector3 endPosition = target.position + localEnd;

        lineRenderer.positionCount = 2;
        lineRenderer.SetPosition(0, startPosition);
        lineRenderer.SetPosition(1, endPosition);

        lineRenderers[index] = lineRenderer;
    }

    void Update()
    {
        speed = (speed + 5) % 500;
        basePosition.z = speed;
        fieldLineBase.SetPosition(0, basePosition);
        
    }
}



