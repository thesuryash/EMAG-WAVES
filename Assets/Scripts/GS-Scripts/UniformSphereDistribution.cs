using UnityEngine;
using System.Collections.Generic;

public class UniformSphereDistribution : MonoBehaviour
{
    [SerializeField] private GameObject sphere;
    //[SerializeField] private LineRenderer linePrefab;
    private List<LineRenderer> lineInstances;
    public int nSample = 100;
    private List<Vector3> surfacePoints;

    
    [SerializeField] public GameObject pointPlaceholder;
    private List<GameObject> points__;


    private Vector3 lastPosition;
    private Vector3 lastScale;

    void Start()
    {
        points__ = new List<GameObject>();
        lineInstances = new List<LineRenderer>();
        if (sphere == null)
        {
            Debug.LogError("Sphere GameObject is not assigned!");
            return;
        }

        for (int i = 0; i < nSample; i++)
        {
            //LineRenderer lineInstance = Instantiate(linePrefab);
            //lineInstance.gameObject.SetActive(false);
            //lineInstances.Add(lineInstance);

            //points__[i].SetActive(false);
            GameObject pp = Instantiate(pointPlaceholder);
            points__.Add(pp);  // Add the instantiated object to the list first
            points__[i].SetActive(false);
            Debug.Log(points__.Count);
        }

        surfacePoints = new List<Vector3>(nSample);
        lastPosition = sphere.transform.position;
        lastScale = sphere.transform.localScale;
        FibonacciSphere();
    }

   void FibonacciSphere()
{
    //surfacePoints.Clear();
    float phi = Mathf.PI * (3 - Mathf.Sqrt(5));  // Golden angle approximation
    float radius = sphere.transform.localScale.x * 0.5f;  // Assumes the sphere has a uniform scale

    for (int i = 0; i < nSample; i++)
    {
        float y = 1 - (i / (float)(nSample - 1)) * 2;  // Normalize y to be between -1 and 1
        float radiusAtY = Mathf.Sqrt(1 - y * y);  // Radius at height y

        float theta = phi * i;  // Incremental angle

        float x = Mathf.Cos(theta) * radiusAtY;
        float z = Mathf.Sin(theta) * radiusAtY;

        Vector3 point = new Vector3(x, y, z) * radius;  // Scale point by the radius of the sphere
        surfacePoints.Add(point);

        points__[i].transform.position = sphere.transform.position + point;  // Offset by sphere's position
        points__[i].SetActive(true);
        
        //lineInstances[i].transform.position = sphere.transform.position + point;
        //lineInstances[i].SetPosition(0, sphere.transform.position + point);
        //lineInstances[i].gameObject.SetActive(true);
    }
}


    public List<Vector3> getDistributedPoints()
    {
        return this.surfacePoints;
    }

    void Update()
    {
        // Check if the sphere has moved or been resized
        if (sphere.transform.position != lastPosition || sphere.transform.localScale != lastScale)
        {
            FibonacciSphere();  // Recalculate the point positions
            lastPosition = sphere.transform.position;
            lastScale = sphere.transform.localScale;
        }
    }
}


