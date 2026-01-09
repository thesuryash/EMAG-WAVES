using UnityEngine;

[RequireComponent(typeof(LineRenderer))]
public class CircleRenderer : MonoBehaviour
{
    [Header("Circle Settings")]
    public float radius = 0.5f;
    [Range(3, 512)] public int segments = 100;

    [Header("References")]
    [SerializeField] private Transform centerObj;          // where the circle is centered
    [SerializeField] private Transform normalSource;       // object whose forward is the rotating arrow / panel normal

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
        DrawCircle();
    }

    public void DrawCircle()
    {
        if (centerObj == null || normalSource == null) return;

        Vector3 center = centerObj.position;

        // Field direction (dashed red line) in world space
        Vector3 fieldDir = Vector3.right;

        // Normal direction (rotating arrow / panel normal)
        Vector3 normalDir = normalSource.forward;

        // Project both onto XZ plane
        fieldDir.y = 0f;
        normalDir.y = 0f;

        if (normalDir.sqrMagnitude < 1e-6f) return;

        fieldDir.Normalize();
        normalDir.Normalize();

        // Angles in XZ
        float fieldAngle = Mathf.Atan2(fieldDir.z, fieldDir.x);
        float normalAngle = Mathf.Atan2(normalDir.z, normalDir.x);

        // Direction from field -> normal (sign tells which side)
        float angleRange = Mathf.DeltaAngle(fieldAngle * Mathf.Rad2Deg, normalAngle * Mathf.Rad2Deg) * Mathf.Deg2Rad;

        // Your existing theta magnitude (do not change flux logic)
        float theta = Mathf.Abs(GlobalVariables.GetThetaRotation) * Mathf.Deg2Rad;  // or GlobalVariables.theta if that's the one you store

        // Clamp so we don't overshoot the normal direction
        float sweep = Mathf.Clamp(theta, 0f, Mathf.Abs(angleRange)) * Mathf.Sign(angleRange);

        float startRad = fieldAngle;
        float endRad = fieldAngle + sweep;

        int points = Mathf.Max(2, segments);
        lineRenderer.positionCount = points + 1;

        for (int i = 0; i <= points; i++)
        {
            float t = (float)i / points;
            float a = Mathf.Lerp(startRad, endRad, t);

            float x = Mathf.Cos(a) * radius + center.x;
            float z = Mathf.Sin(a) * radius + center.z;

            lineRenderer.SetPosition(i, new Vector3(x, center.y, z));
        }
    }
}
