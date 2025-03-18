using UnityEngine;
using UnityEngine.EventSystems;

public class CameraOrbitQuaternion : MonoBehaviour
{
    public Transform target;
    public float zoomSpeed = 600f;
    public float rotationSpeed = 5f;

    private float distanceToTarget;
    private float currentVerticalAngle = 0f;
    private float currentHorizontalAngle = 0f;

    void Start()
    {
        if (target == null)
        {
            Debug.LogError("Target not set for CameraOrbit script.");
            return;
        }

        // Directly use transform to look at the target and calculate the initial distance
        transform.LookAt(target.position);
        distanceToTarget = Vector3.Distance(transform.position, target.position);

        // Initialize rotation angles based on current orientation
        Vector3 angles = transform.eulerAngles;
        currentVerticalAngle = angles.x;
        currentHorizontalAngle = angles.y;

    }

    void Update()
    {
        if (target == null) return;

        /*if (EventSystem.current.IsPointerOverGameObject())
        {
            return;
        }
        */

        // Zoom in/out with mouse scroll wheel
        float scroll = Input.GetAxis("Mouse ScrollWheel");
        distanceToTarget -= scroll * zoomSpeed;
        distanceToTarget = Mathf.Min(Mathf.Max(1f, distanceToTarget), 500f); // Prevent camera from going inside the target or too far

        /*
        if (!PanelInteraction.isMouseinPlayArea)
        {
            return;
        }
        */


        // Rotate around object with mouse drag
        if (Input.GetMouseButton(1))
        {
            currentHorizontalAngle += Input.GetAxis("Mouse X") * rotationSpeed;
            currentVerticalAngle += Input.GetAxis("Mouse Y") * rotationSpeed;
            currentVerticalAngle = Mathf.Clamp(currentVerticalAngle, -90f, 89f); // Clamp to prevent flipping
        }

        // Calculate new position
        Quaternion rotation = Quaternion.Euler(-currentVerticalAngle, currentHorizontalAngle, 0);
        Vector3 positionOffset = rotation * new Vector3(0, 0, -distanceToTarget);
        transform.position = target.position + positionOffset;

        // Ensure the camera always looks at the target
        transform.LookAt(target);
    }
}
