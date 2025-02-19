using UnityEngine;
using System.Collections.Generic;

public class RuntimeTransformGizmo : MonoBehaviour
{
    // List of objects that can be selected and moved
    public List<Transform> targetObjects;

    // Currently selected object
    private Transform selectedObject;

    // Enum to identify the selected axis
    private enum Axis { None, X, Y, Z }
    private Axis selectedAxis = Axis.None;

    // GameObjects for the axes
    private GameObject xAxisObject;
    private GameObject yAxisObject;
    private GameObject zAxisObject;

    // LineRenderers for the axes
    private LineRenderer xAxisRenderer;
    private LineRenderer yAxisRenderer;
    private LineRenderer zAxisRenderer;

    // Colors for the axes
    private Color xAxisColor = Color.red;
    private Color yAxisColor = Color.green;
    private Color zAxisColor = Color.blue;

    // Main camera reference
    private Camera mainCamera;

    // Previous mouse position for calculating movement
    private Vector3 previousMousePosition;

    void Start()
    {
        mainCamera = Camera.main;
        CreateGizmo();
        HideGizmo();
    }

    void Update()
    {
        HandleInput();

        if (selectedObject != null)
        {
            UpdateGizmoPosition();
        }
    }

    // Creates the gizmo with Line Renderers and Colliders
    void CreateGizmo()
    {
        // X-Axis
        xAxisObject = new GameObject("XAxis");
        xAxisObject.transform.SetParent(transform);
        xAxisRenderer = xAxisObject.AddComponent<LineRenderer>();
        SetupAxisRenderer(xAxisRenderer, xAxisColor, Vector3.right);

        // Add collider to X-Axis
        AddAxisCollider(xAxisObject, new Vector3(0.5f, 0, 0), new Vector3(1f, 0.1f, 0.1f));

        // Y-Axis
        yAxisObject = new GameObject("YAxis");
        yAxisObject.transform.SetParent(transform);
        yAxisRenderer = yAxisObject.AddComponent<LineRenderer>();
        SetupAxisRenderer(yAxisRenderer, yAxisColor, Vector3.up);

        // Add collider to Y-Axis
        AddAxisCollider(yAxisObject, new Vector3(0, 0.5f, 0), new Vector3(0.1f, 1f, 0.1f));

        // Z-Axis
        zAxisObject = new GameObject("ZAxis");
        zAxisObject.transform.SetParent(transform);
        zAxisRenderer = zAxisObject.AddComponent<LineRenderer>();
        SetupAxisRenderer(zAxisRenderer, zAxisColor, Vector3.forward);

        // Add collider to Z-Axis
        AddAxisCollider(zAxisObject, new Vector3(0, 0, 0.5f), new Vector3(0.1f, 0.1f, 1f));
    }

    // Sets up a LineRenderer for an axis
    void SetupAxisRenderer(LineRenderer renderer, Color color, Vector3 direction)
    {
        renderer.material = new Material(Shader.Find("Sprites/Default"));
        renderer.startColor = color;
        renderer.endColor = color;
        renderer.startWidth = 0.02f;
        renderer.endWidth = 0.02f;
        renderer.positionCount = 2;
        renderer.useWorldSpace = false;
        renderer.SetPosition(0, Vector3.zero);
        renderer.SetPosition(1, direction);
    }

    // Adds a BoxCollider to an axis
    void AddAxisCollider(GameObject axisObject, Vector3 center, Vector3 size)
    {
        BoxCollider collider = axisObject.AddComponent<BoxCollider>();
        collider.isTrigger = true;
        collider.center = center;
        collider.size = size;
    }

    // Handles user input for selecting objects and moving them
    void HandleInput()
    {
        if (Input.GetMouseButtonDown(0))
        {
            // Raycast from the mouse position
            Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            // Check if a target object is selected
            if (Physics.Raycast(ray, out hit))
            {
                if (targetObjects.Contains(hit.transform))
                {
                    selectedObject = hit.transform;
                    transform.position = selectedObject.position;
                    ShowGizmo();
                    previousMousePosition = Input.mousePosition;
                }
                // Check if an axis is selected
                else if (selectedObject != null)
                {
                    if (hit.collider.gameObject == xAxisObject)
                    {
                        selectedAxis = Axis.X;
                        previousMousePosition = Input.mousePosition;
                    }
                    else if (hit.collider.gameObject == yAxisObject)
                    {
                        selectedAxis = Axis.Y;
                        previousMousePosition = Input.mousePosition;
                    }
                    else if (hit.collider.gameObject == zAxisObject)
                    {
                        selectedAxis = Axis.Z;
                        previousMousePosition = Input.mousePosition;
                    }
                }
            }
        }

        // Move the selected object along the selected axis
        if (Input.GetMouseButton(0) && selectedAxis != Axis.None && selectedObject != null)
        {
            Vector3 mouseDelta = Input.mousePosition - previousMousePosition;
            float moveAmount = (mouseDelta.x + mouseDelta.y) * 0.01f;
            Vector3 move = Vector3.zero;

            switch (selectedAxis)
            {
                case Axis.X:
                    move = new Vector3(moveAmount, 0, 0);
                    break;
                case Axis.Y:
                    move = new Vector3(0, moveAmount, 0);
                    break;
                case Axis.Z:
                    move = new Vector3(0, 0, moveAmount);
                    break;
            }

            selectedObject.position += move;
            transform.position = selectedObject.position;
            previousMousePosition = Input.mousePosition;
        }

        // Reset the selected axis when the mouse button is released
        if (Input.GetMouseButtonUp(0))
        {
            selectedAxis = Axis.None;
        }
    }

    // Updates the gizmo's position to match the selected object's position
    void UpdateGizmoPosition()
    {
        transform.position = selectedObject.position;
    }

    // Shows the gizmo
    void ShowGizmo()
    {
        xAxisObject.SetActive(true);
        yAxisObject.SetActive(true);
        zAxisObject.SetActive(true);
    }

    // Hides the gizmo
    void HideGizmo()
    {
        xAxisObject.SetActive(false);
        yAxisObject.SetActive(false);
        zAxisObject.SetActive(false);
    }
}

