using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class CameraSystem : MonoBehaviour
{
    // Start is called before the first frame update
    [SerializeField] private bool useEdgeScrolling = false;
    [SerializeField] private CinemachineVirtualCamera cinemachineVirtualCamera;
    [SerializeField] private float fieldOfViewMax = 80;
    [SerializeField] private float fieldOfViewMin = 10;

    public Transform target;
    public float zoomSpeed = 6f;
    public float rotationSpeed = 5f;

    private float distanceToTarget;
    private float currentVerticalAngle = 0f;
    private float currentHorizontalAngle = 0f;

    private float targetFieldOfView = 20f; 
    private bool dragPanActive;
    private Vector2 lastMousePosition;


    void Start()
    {

    }

    // Update is called once per frame
    void Update()
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


        HandleCameraMovement();
        HandleCameraRotation();
        HandleCameraMovementEdgeScrolling();
        HandleCameraMovementDragPan();
        HandleCameraZoom();

    }
    private void HandleCameraMovement()
    {
        Vector3 inputDir = new Vector3(0, 0, 0);

        if (Input.GetKey(KeyCode.W)) inputDir.z = +1f;
        if (Input.GetKey(KeyCode.S)) inputDir.z = -1f;
        if (Input.GetKey(KeyCode.A)) inputDir.x = -1f;
        if (Input.GetKey(KeyCode.D)) inputDir.x = +1f;

        /*  Debug.Log(Input.mousePresent);*/

        Vector3 moveDir = transform.forward * inputDir.z + transform.right * inputDir.x;
        float moveSpeed = 5f;
        transform.position += moveDir * moveSpeed * Time.deltaTime;



    }
    private void HandleCameraMovementEdgeScrolling()
    {

        Vector3 inputDir = new Vector3(0, 0, 0);

        if (useEdgeScrolling)
        {

            int edgeScrollSize = 20;
            if (Input.mousePosition.x < edgeScrollSize) inputDir.x = -1f;
            if (Input.mousePosition.y > edgeScrollSize) inputDir.z = -1f;
            if (Input.mousePosition.x > Screen.width - edgeScrollSize) inputDir.x = +1f;
            if (Input.mousePosition.y > Screen.height - edgeScrollSize) inputDir.z = +1f;
        }
        Vector3 moveDir = transform.forward * inputDir.z + transform.right * inputDir.x;
        float moveSpeed = 5f;
        transform.position += moveDir * moveSpeed * Time.deltaTime;


    }

    private void HandleCameraZoom()
    {

        if(Input.mouseScrollDelta.y > 0)
        {
            targetFieldOfView -= 5;
        }
        if(Input.mouseScrollDelta.y < 0)
        {
            targetFieldOfView += 5;
        }
        targetFieldOfView = Mathf.Clamp(targetFieldOfView, fieldOfViewMin, fieldOfViewMax);

        float zoomSpeed = 10f;
        Mathf.Lerp(cinemachineVirtualCamera.m_Lens.OrthographicSize, targetFieldOfView, Time.deltaTime * zoomSpeed);
        cinemachineVirtualCamera.m_Lens.OrthographicSize = targetFieldOfView;
        
    }

    private void HandleCameraMovementDragPan()
    {
        Vector3 inputDir = new Vector3(0, 0, 0);


        //Drag
        if (Input.GetMouseButtonDown(0))
        {
            dragPanActive = true;
            lastMousePosition = Input.mousePosition;
        }
        if (Input.GetMouseButtonUp(0))
        {
            dragPanActive = false;
        }

        if (dragPanActive)
        {
            Vector2 mouseMovementDelta = (Vector2)Input.mousePosition - lastMousePosition;

            float dragPanSpeed = 2f;
            inputDir.x = mouseMovementDelta.x * dragPanSpeed;
            inputDir.y = mouseMovementDelta.y * dragPanSpeed;

            lastMousePosition = Input.mousePosition;
        }

        Vector3 moveDir = transform.forward * inputDir.z + transform.right * inputDir.x;
        float moveSpeed = 5f;
        transform.position += moveDir * moveSpeed * Time.deltaTime;
    }

    private void HandleCameraRotation()
    {
        //Rotation
        float rotateSpeed = 60f;
        float rotateDir = 0f;
        if (Input.GetKey(KeyCode.Q)) rotateDir = +1f;
        if (Input.GetKey(KeyCode.E)) rotateDir = -1f;

        transform.eulerAngles += new Vector3(0, rotateDir * rotateSpeed * Time.deltaTime, 0);



    }
}
