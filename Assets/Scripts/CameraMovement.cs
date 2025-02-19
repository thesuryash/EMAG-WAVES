using UnityEngine;

namespace RuntimeSceneGizmo
{
	public class CameraMovement : MonoBehaviour
	{
		[SerializeField] private Camera mainCamera; // Serialized field for the camera
		[SerializeField] private Transform targetObject; // Serialized field for the target object
		[SerializeField] private float sensitivity = 0.5f;

		private Vector3 prevMousePos;

		private void Awake()
		{
			// Check if mainCamera is assigned, otherwise use Camera.main
			if (mainCamera == null)
			{
				mainCamera = Camera.main;
			}
		}

		private void Update()
		{
			if (Input.GetMouseButtonDown(0))
			{
				prevMousePos = Input.mousePosition;
			}
			else if (Input.GetMouseButton(0))
			{
				Vector3 mousePos = Input.mousePosition;
				Vector2 deltaPos = (mousePos - prevMousePos) * sensitivity;

				// Rotate around the target object if assigned
				if (targetObject != null)
				{
					mainCamera.transform.LookAt(targetObject);
				}

				// Rotate based on mouse delta
				mainCamera.transform.RotateAround(targetObject != null ? targetObject.position : transform.position, Vector3.up, deltaPos.x);
				mainCamera.transform.RotateAround(targetObject != null ? targetObject.position : transform.position, Vector3.right, -deltaPos.y);

				// Clamp vertical rotation
				float rotX = mainCamera.transform.eulerAngles.x;
				while (rotX > 180f)
				{
					rotX -= 360f;
				}
				while (rotX < -180f)
				{
					rotX += 360f;
				}
				mainCamera.transform.eulerAngles = new Vector3(Mathf.Clamp(rotX, -89.8f, 89.8f), mainCamera.transform.eulerAngles.y, 0f);

				prevMousePos = mousePos;
			}
		}
	}
}



