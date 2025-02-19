//using RuntimeGizmos;
using System.Collections;
using System.Collections.Generic;
//using UnityEditor.Experimental.GraphView;
using UnityEngine;

public class LRDirection : MonoBehaviour

{
    [SerializeField] private GameObject parent;
    private Vector3 center_;
    private LineRenderer lr;
    private Vector3 direction;
    // Start is called before the first frame update
    void Start()
    {
        lr = GetComponent<LineRenderer>();
        calibrateDirection();

        Vector3 length = lr.GetPosition(0) - lr.GetPosition(1);

        lr.SetPosition(1, length.magnitude * direction);
    }

    // Update is called once per frame
    void Update()
    {
        calibrateDirection();


    }

    void calibrateDirection()
    {
        SphereCollider sc = parent.GetComponent<SphereCollider>();
        if (sc != null)
        {
            center_ = sc.center;

        }
        direction = center_ - transform.position;
    }
}
