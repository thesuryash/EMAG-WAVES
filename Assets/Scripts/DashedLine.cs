using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DashedLine : MonoBehaviour
{
    public LineRenderer lineRenderer;
    public int segment = 20;
    public float dashLength = 00.5f;
    public float gapLength = 0.2f;
    
    // Start is called before the first frame update
    void Start()
    {
        lineRenderer.positionCount = segment * 2;
        Vector3 start = lineRenderer.GetPosition(0);
        Vector3 end = lineRenderer.GetPosition(1);
        Vector3 direction = (end - start).normalized;

        for (int i = 0; i < segment; i++) {
            float dist = (dashLength + gapLength) * i;
            lineRenderer.SetPosition(i*2, start + direction * dist);
            lineRenderer.SetPosition(i * 2 + 1, start + direction * (dist + dashLength));
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
