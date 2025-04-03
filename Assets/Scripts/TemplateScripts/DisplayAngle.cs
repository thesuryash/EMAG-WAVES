using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DisplayAngle : MonoBehaviour
{
    [SerializeField] private Vector3 tail1;
    [SerializeField] private Vector3 tail2;
    [SerializeField] private GameObject center;
    private AngleBetweenVectors angle;
    // Start is called before the first frame update
    void Start()
    {

        angle = new AngleBetweenVectors(tail1, tail2, 10f);
        angle.ShowAngleSector();
    }

    // Update is called once per frame
    void Update()
    {
        angle.ShowAngleSector();

    }
}
