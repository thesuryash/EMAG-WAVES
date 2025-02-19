using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ReplicateLines : MonoBehaviour
{
    [SerializeField] private LineRenderer baseLine;
    [SerializeField] private Transform parent;
    //[SerializeField] private float density;
    private Vector3 pos;
    private List<LineRenderer> lines;
    private List<Vector3> linePositions;

    private void StepSizeByEField(float field)
    {
        if (field == 0)
        {
            baseLine.enabled = false;
        }
 
    }
    private void Awake()
    {
        pos = baseLine.transform.position;

        Vector3 startPosX = new Vector3(pos.x-50f, pos.y, pos.z);
        Vector3 endPosX = new Vector3(pos.x+50f, pos.y, pos.z);
        Vector3 stepX = new Vector3(4f, 0, 0);

        lines = new List<LineRenderer>();
        linePositions = new List<Vector3>();

        for (Vector3 i = startPosX; i.x <= endPosX.x; i += stepX)
        {
            Vector3 startPosY = new Vector3(i.x, pos.y-50f, pos.z);
            Vector3 endPosY = new Vector3(i.x, pos.y+50f, pos.z);
            Vector3 stepY = new Vector3(0, 4f, 0);

            for (Vector3 j = startPosY; j.y <= endPosY.y; j += stepY)
            {
                LineRenderer lr = Instantiate(baseLine);
                lr.transform.position = new Vector3(i.x, j.y, pos.z);
                /*                lr.startColor = new Color(0, 1, 0, 1 / 50 * i.x);
                                lr.endColor = new Color(0, 1, 0, 1 / 50 * i.x);*/
                lr.startColor = new Color(0.537f, 0.812f, 0.941f, 1 / 50 * i.x);
                lr.endColor = new Color(0.537f, 0.812f, 0.941f, 1 / 50 * i.x);
                Debug.Log("***************** StartColor: " + (lr.material.color.a - 1 / 50).ToString());
                Debug.Log("***************** EndColor: " + (lr.material.color.a - 1 / 50).ToString());
                /*lr.SetColors(new Color(0,0,1,1/50 * i.x), new Color(0, 0, 1, 1 / 50 * i.x));*/
                /* lr.material.SetColor("_Color", new Color(lr.material.color.r, lr.material.color.g, lr.material.color.b, lr.material.color.a - 1/50));
                 Debug.Log("*****************" + (lr.material.color.a - 1 / 50).ToString());*/

                lines.Add(lr);
                linePositions.Add(new Vector3(i.x, j.y, pos.z));

                lr.transform.SetParent(parent);
            }
        }
    }

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }
}



