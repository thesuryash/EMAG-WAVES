using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(LineRenderer))]
public class CircleRenderer : MonoBehaviour
{
    [Header("Circle Settings")]
    public float radius = 0.5f;
    [Range(3, 512)] public int segments = 100;

    [SerializeField] private GameObject centerObj;
    [SerializeField] private Slider rotationSlider;
    [SerializeField] private bool debugDraw = false;

    private LineRenderer lineRenderer;

    void Awake()
    {
        lineRenderer = GetComponent<LineRenderer>();
        lineRenderer.useWorldSpace = true;
        lineRenderer.startWidth = 0.05f;
        lineRenderer.endWidth = 0.05f;
    }

    void Update()
    {
        if (rotationSlider == null || centerObj == null) return;

        // Get the actual area vector from the surface's rotation
        Vector3 areaDir = centerObj.transform.up;

        // Measure angle from +X to areaDir using right-handed convention
        float signedTheta = Vector3.SignedAngle(Vector3.right, areaDir, Vector3.up); // degrees

        if (debugDraw)
        {
            Vector3 center = centerObj.transform.position;
            Vector3 startDir = Vector3.right;
            Debug.DrawLine(center, center + startDir * radius, Color.blue);   // +X reference
            Debug.DrawLine(center, center + areaDir * radius, Color.green);   // area vector
        }

        DrawArcFromSignedAngle(signedTheta);
    }

    public void DrawArcFromSignedAngle(float signedTheta)
    {
        float angleRangeDeg = Mathf.Abs(signedTheta);
        float startRad = 0f; // anchored to +X axis
        float sweepRad = angleRangeDeg * Mathf.Deg2Rad;

        int points = Mathf.Max(2, Mathf.CeilToInt(segments * (angleRangeDeg / 360f)));
        lineRenderer.positionCount = points + 1;

        Vector3 center = centerObj.transform.position;

        for (int i = 0; i <= points; i++)
        {
            float t = i / (float)points;
            float angleRad = signedTheta >= 0f
                ? startRad - t * sweepRad   // counterclockwise for positive angles
                : startRad + t * sweepRad;  // clockwise for negative angles

            float x = Mathf.Cos(angleRad) * radius + center.x;
            float z = Mathf.Sin(angleRad) * radius + center.z;

            lineRenderer.SetPosition(i, new Vector3(x, center.y, z));
        }
    }
}