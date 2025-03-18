using UnityEngine;
using UnityEngine.UI;

public class CameraOrbitQuaternionn : MonoBehaviour
{
    public Transform target;
    public float zoomSpeedMouse = 60f;
    public float zoomSpeedTouch = 0.6f;

    public float rotationSpeed = Mathf.PI / 2;

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

    private bool isNewPinch = true; // Flag to detect start of a new pinch
    private bool touchPan = true;

    void Update()
    {
        if (target == null) return;

        float scroll = Input.GetAxis("Mouse ScrollWheel");
        distanceToTarget -= scroll * zoomSpeedMouse;
        distanceToTarget = Mathf.Clamp(distanceToTarget, 5f, 200f);


        /*if (Input.touchCount == 2)
        {
            touchPan = false;
            Touch touch1 = Input.GetTouch(0);
            Touch touch2 = Input.GetTouch(1);

            float deltaMagnitudeDiff = (touch2.deltaPosition - touch1.deltaPosition).magnitude;
            distanceToTarget -= deltaMagnitudeDiff * zoomSpeedTouch * Time.deltaTime;
            distanceToTarget = Mathf.Clamp(distanceToTarget, 10f, 200f);

        }
        else if (Input.touchCount == 0)
        {
            touchPan = true;
        }*/


        if (Input.touchCount == 2)
        {
            touchPan = false;
            Touch touch1 = Input.GetTouch(0);
            Touch touch2 = Input.GetTouch(1);

            Vector2 touch1PrevPos = touch1.position - touch1.deltaPosition;
            Vector2 touch2PrevPos = touch2.position - touch2.deltaPosition;

            if (isNewPinch)
            {
                // Reset the initial positions to avoid sudden jumps in zoom
                touch1PrevPos = touch1.position;
                touch2PrevPos = touch2.position;
                isNewPinch = false; // Reset the flag after setting initial positions
            }

            float prevTouchDeltaMag = (touch1PrevPos - touch2PrevPos).magnitude;
            float touchDeltaMag = (touch1.position - touch2.position).magnitude;

            float deltaMagnitudeDiff = prevTouchDeltaMag - touchDeltaMag;

            //float deltaMagnitudeDiff = (touch2.deltaPosition - touch1.deltaPosition).magnitude;

            distanceToTarget -= deltaMagnitudeDiff * zoomSpeedTouch * Time.deltaTime;
            distanceToTarget = Mathf.Clamp(distanceToTarget, 15f, 200f);
        }
        /*
                else if (Input.touchCount > 0)
                {
                    isNewPinch = false; // Do not reset until all fingers are removed
                }*/

        else if (Input.touchCount == 0)
        {
            isNewPinch = true; // Reset when less than two touches are detected
            touchPan = true;
        }


        if (!PanelInteraction.isMouseinPlayArea || SliderInteraction.isMouseOverSlider)
        {
            return;
        }

        if (touchPan && Input.GetMouseButton(0))
        {
            currentHorizontalAngle += Input.GetAxis("Mouse X") * rotationSpeed;
            currentVerticalAngle -= Input.GetAxis("Mouse Y") * rotationSpeed;
            currentVerticalAngle = Mathf.Clamp(currentVerticalAngle, -89.9f, 89.9f);
        }

        Quaternion rotation = Quaternion.Euler(currentVerticalAngle, currentHorizontalAngle, 0);
        Vector3 positionOffset = rotation * new Vector3(0, 0, -distanceToTarget);
        transform.position = target.position + positionOffset;

        transform.LookAt(target);
    }




}







//using UnityEngine;

//public class CameraOrbitQuaternionn : MonoBehaviour
//{
//    public Transform target;
//    public float zoomSpeedMouse = 60f;
//    public float zoomSpeedTouch = 0.6f;

//    public float rotationSpeed = Mathf.PI / 2;

//    private float distanceToTarget;
//    private float currentVerticalAngle = 0f;
//    private float currentHorizontalAngle = 0f;
//    public Vector3 resetPosition;
//    [SerializeField] public GameObject cameraSystem;


//    private void Awake()
//    {
//        cameraSystem.transform.position = resetPosition;
//    }
//    void Start()
//    {
//        if (target == null)
//        {
//            cameraSystem.transform.position = resetPosition;

//            Debug.LogError("Target not set for CameraOrbit script.");
//            return;
//        }

//        // Directly use transform to look at the target and calculate the initial distance
//        transform.LookAt(target.position);
//        distanceToTarget = Vector3.Distance(transform.position, target.position);

//        // Initialize rotation angles based on current orientation
//        Vector3 angles = transform.eulerAngles;
//        currentVerticalAngle = angles.x;
//        currentHorizontalAngle = angles.y;

//        ScreenControls();

//    }

//    private bool isNewPinch = true; // Flag to detect start of a new pinch
//    private bool touchPan = true;



//    void Update()
//    {
//        ScreenControls();
//    }

//    void ScreenControls()
//    {
//        if (target == null) return;

//        float scroll = Input.GetAxis("Mouse ScrollWheel");
//        distanceToTarget -= scroll * zoomSpeedMouse;
//        distanceToTarget = Mathf.Clamp(distanceToTarget, 5f, 200f);


//        /*if (Input.touchCount == 2)
//        {
//            touchPan = false;
//            Touch touch1 = Input.GetTouch(0);
//            Touch touch2 = Input.GetTouch(1);

//            float deltaMagnitudeDiff = (touch2.deltaPosition - touch1.deltaPosition).magnitude;
//            distanceToTarget -= deltaMagnitudeDiff * zoomSpeedTouch * Time.deltaTime;
//            distanceToTarget = Mathf.Clamp(distanceToTarget, 10f, 200f);

//        }
//        else if (Input.touchCount == 0)
//        {
//            touchPan = true;
//        }*/


//        if (Input.touchCount == 2)
//        {
//            touchPan = false;
//            Touch touch1 = Input.GetTouch(0);
//            Touch touch2 = Input.GetTouch(1);

//            Vector2 touch1PrevPos = touch1.position - touch1.deltaPosition;
//            Vector2 touch2PrevPos = touch2.position - touch2.deltaPosition;

//            if (isNewPinch)
//            {
//                // Reset the initial positions to avoid sudden jumps in zoom
//                touch1PrevPos = touch1.position;
//                touch2PrevPos = touch2.position;
//                isNewPinch = false; // Reset the flag after setting initial positions
//            }

//            float prevTouchDeltaMag = (touch1PrevPos - touch2PrevPos).magnitude;
//            float touchDeltaMag = (touch1.position - touch2.position).magnitude;

//            float deltaMagnitudeDiff = prevTouchDeltaMag - touchDeltaMag;

//            //float deltaMagnitudeDiff = (touch2.deltaPosition - touch1.deltaPosition).magnitude;

//            distanceToTarget -= deltaMagnitudeDiff * zoomSpeedTouch * Time.deltaTime;
//            distanceToTarget = Mathf.Clamp(distanceToTarget, 15f, 200f);
//        }
//        /*
//                else if (Input.touchCount > 0)
//                {
//                    isNewPinch = false; // Do not reset until all fingers are removed
//                }*/

//        else if (Input.touchCount == 0)
//        {
//            isNewPinch = true; // Reset when less than two touches are detected
//            touchPan = true;
//        }


//        if (!PanelInteraction.isMouseinPlayArea || SliderInteraction.isMouseOverSlider)
//        {
//            return;
//        }

//        if (touchPan && Input.GetMouseButton(0))
//        {
//            currentHorizontalAngle += Input.GetAxis("Mouse X") * rotationSpeed;
//            currentVerticalAngle -= Input.GetAxis("Mouse Y") * rotationSpeed;
//            currentVerticalAngle = Mathf.Clamp(currentVerticalAngle, -89.9f, 89.9f);
//        }

//        Quaternion rotation = Quaternion.Euler(currentVerticalAngle, currentHorizontalAngle, 0);
//        Vector3 positionOffset = rotation * new Vector3(0, 0, -distanceToTarget);
//        //Vector3 positionOffset = rotation * new Vector3(-distanceToTarget,0, 0);

//        transform.position = target.position + positionOffset;

//        transform.LookAt(target);
//    }
//}



