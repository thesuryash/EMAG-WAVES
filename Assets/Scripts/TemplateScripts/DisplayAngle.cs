using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DisplayAngle : MonoBehaviour
{
    [SerializeField] private GameObject tail1;
    [SerializeField] private GameObject tail2;
    [SerializeField] private GameObject center;
    private AngleBetweenVectors angle;
    // Start is called before the first frame update
    void Start()
    {
        Vector3 v1 = tail1.transform.forward;
        Vector3 v2 = tail2.transform.forward;

        angle = new AngleBetweenVectors(v1, v2, 10f);
        angle.ShowAngleSector();
    }

    // Update is called once per frame
    void Update()
    {
        angle.ShowAngleSector();

    }
}
