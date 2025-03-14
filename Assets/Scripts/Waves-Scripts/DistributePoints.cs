using UnityEngine;
using UnityEngine.UIElements;

public class DistributedWavePoints : MonoBehaviour
{
    [SerializeField]
    private GameObject pointPrefab; // Assign your point prefab here
    [SerializeField]
    private GameObject plane1;
    [SerializeField]
    private GameObject plane2;
    [SerializeField] private GameObject parent;

    private int stepSize = 0; // Step size to control point spacing


    private float k = GlobalVariables.k;
    private float omega = GlobalVariables.omega;
    private float E0 = GlobalVariables.E0;
    private float B0 = GlobalVariables.B0;

    float length;


    void Start()
    {

        // Check if plane1 and plane2 are assigned
        if (plane1 == null || plane2 == null)
        {
            Debug.LogError("Planes are not assigned! Please assign plane1 and plane2 in the Inspector.");
            return;
        }

        // Calculate the distance between the planes
        length = Mathf.Abs(plane1.transform.position.x - plane2.transform.position.x);


        stepSize = (int)(length / 1);
        // Check for valid input
        if (length <= 0 || stepSize <= 0)
        {
            Debug.LogError("Invalid length or step size. Please check your setup.");
            return;
        }

        // Log the calculated length
        Debug.Log($"Calculated Length: {length}");

        // Generate points
        GeneratePoints(length);
    }
    private void Update()
    {
        GeneratePoints(length);
    }

    private void GeneratePoints(float length)
    {
        int pointCount = 0;
        float  x0 = pointPrefab.transform.position.x;
        float y0 = pointPrefab.transform.position.y;
        float z0 = pointPrefab.transform.position.z;
        // Iterate through a 3D grid to create points
        for (float x = x0-2*length; x < x0+ 2 * length; x += stepSize)
        {
            for (float y = y0- length* 0.6f; y < y0+length * 0.6; y += stepSize)
            {
                for (float z = z0 - length * 0.6f; z < z0+0.6 * length; z += stepSize)
                {
                    // Create a point at the calculated position
                    float X_offset = x - x0;
                    float Y_offsetE = E0 * Mathf.Sin(k *X_offset - omega * Time.deltaTime);
                    float Z_offset = z;

                    Vector3 position = new Vector3(x,y+Y_offsetE,Z_offset);
                    Debug.Log(position);
                    if (pointPrefab != null)
                    {
                        GameObject point = Instantiate(pointPrefab);
                        point.transform.SetParent(transform);
                        point.transform.position = position;
                        Arrow arr = new Arrow(point);
                        pointCount++;
                        Debug.Log($"Point {pointCount} created at: {position}");
                    }
                }
            }
        }

        // Log the total points created
        Debug.Log($"Total Points Created: {pointCount}");
    }
}
