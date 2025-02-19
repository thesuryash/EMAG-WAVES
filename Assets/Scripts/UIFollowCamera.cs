using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIFollowCamera : MonoBehaviour
{
    public Camera miniCamera;
    public Vector3 offset = new Vector3(0, 0, 2);
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        transform.position = miniCamera.transform.position + miniCamera.transform.forward * offset.z + miniCamera.transform.right * offset.x + miniCamera.transform.up * offset.y;
        transform.rotation = Quaternion.LookRotation(transform.position - miniCamera.transform.position);
    }
}
