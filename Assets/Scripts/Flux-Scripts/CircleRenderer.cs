using UnityEngine;

[RequireComponent(typeof(LineRenderer))]
public class CircleRenderer : MonoBehaviour
{
    [Header("Circle Settings")]
    public float radius = 0.5f;             // Distance from center to circle edge
    [Range(3, 512)] public int segments = 100;  // Number of line segments

    [Header("Sector Settings")]
    [Range(0f, 360f)] public float startAngle = 0f;   // In degrees
    [Range(0f, 360f)] public float endAngle = 180f;   // In degrees

    [SerializeField] public GameObject centerObj;

    private LineRenderer lineRenderer;

    void Awake()
    {
        lineRenderer = GetComponent<LineRenderer>();
        lineRenderer.useWorldSpace = true;
        DrawCircle();
        lineRenderer.startWidth = 0.05f;
        lineRenderer.endWidth = 0.05f;

    }

    void Start()
    {
        endAngle = GlobalVariables.GetThetaRotation ;
        DrawCircle();
    }

void Update()
    {
        endAngle = GlobalVariables.GetThetaRotation ; 
        DrawCircle();
    }

    void OnValidate()
    {
        // Automatically update in Editor when values change
        if (Application.isPlaying && lineRenderer != null)
            DrawCircle();
    }

    public void DrawCircle()
    {
        float theta = GlobalVariables.theta * Mathf.Deg2Rad;  // θ in radians

        // Arc starts at +X (field direction)
        float startRad = 0f;
        float endRad = theta;
        float angleRange = endRad - startRad;

        int points = Mathf.Max(2, Mathf.CeilToInt(segments * (angleRange / (2 * Mathf.PI))));
        lineRenderer.positionCount = points + 1;

        Vector3 center = centerObj.transform.position;

        for (int i = 0; i <= points; i++)
        {
            float t = i / (float)points;
            float angle = startRad + t * angleRange;

            float x = Mathf.Cos(angle) * radius + center.x;
            float z = Mathf.Sin(angle) * radius + center.z;

            lineRenderer.SetPosition(i, new Vector3(x, center.y, z));
        }
    }




}
