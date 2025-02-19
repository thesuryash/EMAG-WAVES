using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraZoom : MonoBehaviour
{
    private float zoom;
    private float zoomMultiplier = 4f;
    private float minZoom = 2f;
    private float maxZoom = 8f;
    private float velocity = 0f;
    private float smoothTime = 0.25f;

    [SerializeField] private Camera cam;
    
    // Start is called before the first frame update
    void Start()
    {
        /*_zoom = _cam.orthographicSize;*/
        zoom = cam.fieldOfView;
    }

    // Update is called once per frame
    void Update()
    {
        float scroll = Input.GetAxis("Mouse ScrollWheel");
        zoom-=scroll * zoomMultiplier;
        zoom = Mathf.Clamp(zoom, minZoom, maxZoom);

        //To smoothly zoom in and out with SmoothDamp()
        cam.orthographicSize = Mathf.SmoothDamp(cam.orthographicSize, zoom, ref velocity, smoothTime);
    }
}
