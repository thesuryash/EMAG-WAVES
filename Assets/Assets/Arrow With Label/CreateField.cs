/*using System.Collections;
using System.Collections.Generic;
using System.Numerics;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.ProBuilder;


public class CreateField : MonoBehaviour
{
    [SerializeField] private GameObject centerObject; // The game object around which points will be distributed
    //[SerializeField] private ProBuilderMesh head;
    //[SerializeField] private ProBuilderMesh Tail;
    [SerializeField] private GameObject prefab;
    private GameObject thisObject;


    public float radius = 5f; // Distance from the center to the points
    public int numberOfPoints = 25; // Total number of points
    public List<UnityEngine.Vector3> pointsAroundCube;

    // Start is called before the first frame update
    private void Awake()
    {
        thisObject = GetComponent<GameObject>();

        UnityEngine.Quaternion fieldDirection = UnityEngine.Quaternion.Euler(90, 90, 0);
        DistributePoints();
        for (int i = 0; i < pointsAroundCube.Count; i++) {
            GameObject go = new GameObject("E " + i);
            go.transform.SetParent(this.transform);
            go.transform.position = pointsAroundCube[i];
            go.transform.rotation = fieldDirection;

            Instantiate(prefab, go.transform.position, go.transform.rotation);

              }
        
    }

    void Start()
    {
        foreach (GameObject arrowMesh in prefab.transform)
        {
            // Find the "head" and "tail" child objects
            ProBuilderMesh head = arrowMesh.transform.Find("head").GetComponent<ProBuilderMesh>();
            ProBuilderMesh tail = arrowMesh.transform.Find("tail").GetComponent<ProBuilderMesh>();

            // Add ProBuilder components to head and tail (assuming ProBuilder is imported)
            if (head != null)
                head.gameObject.AddComponent<ProBuilderMesh>();
            if (tail != null)
                tail.gameObject.AddComponent<ProBuilderMesh>();

            // Create an Arrow object (replace with your actual Arrow class)
            // Adjust the direction vector as needed
            Arrow ar = new Arrow(arrowMesh, head, tail, new UnityEngine.Vector3(0, 0, 1), 1f);
            //ar.transform.SetParent(arrowMesh.transform);
            ar.SetScene();

    }
}

    // Update is called once per frame
    void Update()
    {
        
    }

    private void DistributePoints()
    {
        for (int i = 0; i < numberOfPoints; i++)
        {
            float angle = i * (360f / numberOfPoints); // Calculate the angle
            float radians = angle * Mathf.Deg2Rad; // Convert angle to radians

            // Calculate the position using trigonometry
            float x = centerObject.transform.position.x + radius * Mathf.Cos(radians);
            float y = centerObject.transform.position.y;
            float z = centerObject.transform.position.z + radius * Mathf.Sin(radians);

            UnityEngine.Vector3 point = new UnityEngine.Vector3(x, y, z);
            pointsAroundCube.Add(point);
        }
    }
}
*/