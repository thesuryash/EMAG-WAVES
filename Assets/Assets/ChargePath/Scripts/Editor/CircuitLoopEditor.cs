using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(CircuitLoop))]
public class CircuitLoopEditor : Editor
{
    private CircuitLoop _circuit;

    private void OnEnable()
    {
        _circuit = (CircuitLoop)target;
    }

    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        EditorGUILayout.Space();
        EditorGUILayout.LabelField("Generation Tools", EditorStyles.boldLabel);

        GUILayout.BeginHorizontal();
        if (GUILayout.Button("Generate Ellipse"))
        {
            Undo.RecordObject(_circuit, "Generate Ellipse");
            _circuit.GenerateEllipse();
            SceneView.RepaintAll();
        }
        if (GUILayout.Button("Generate Polygon"))
        {
            Undo.RecordObject(_circuit, "Generate Polygon");
            _circuit.GeneratePolygon();
            SceneView.RepaintAll();
        }
        GUILayout.EndHorizontal();

        if (GUILayout.Button("Snap to Ground (Local Y=0)"))
        {
            Undo.RecordObject(_circuit, "Flatten Path");
            for (int i = 0; i < _circuit.waypoints.Count; i++)
            {
                Vector3 p = _circuit.waypoints[i];
                // Keeps x and z, flattens y relative to the object
                _circuit.waypoints[i] = new Vector3(p.x, 0, p.z);
            }
            _circuit.UpdateLineRenderer();
            SceneView.RepaintAll();
        }
    }

    private void OnSceneGUI()
    {
        if (_circuit.waypoints == null) return;

        // Since the object might be rotated, we need to handle rotation in the editor tools
        Quaternion rotation = _circuit.transform.rotation;

        for (int i = 0; i < _circuit.waypoints.Count; i++)
        {
            // 1. Convert Local Point -> World Point
            Vector3 localPoint = _circuit.waypoints[i];
            Vector3 worldPoint = _circuit.transform.TransformPoint(localPoint);

            Handles.Label(worldPoint + Vector3.up * 0.5f, i.ToString());

            EditorGUI.BeginChangeCheck();

            // 2. Draw Handle in World Space
            Vector3 newWorldPoint = Handles.PositionHandle(worldPoint, rotation);

            if (EditorGUI.EndChangeCheck())
            {
                Undo.RecordObject(_circuit, "Move Path Point");

                // 3. Convert New World Point -> Local Point to save
                _circuit.waypoints[i] = _circuit.transform.InverseTransformPoint(newWorldPoint);

                _circuit.UpdateLineRenderer();
            }
        }
    }
}