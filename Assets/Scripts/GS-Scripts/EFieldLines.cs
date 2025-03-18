using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.ProBuilder;
using UnityEngine.SocialPlatforms;
using UnityEngine.UI;


public class EFieldLines : MonoBehaviour
{
    [SerializeField] public GameObject pointCharge;
    private List<LineRenderer> fieldLines = new List<LineRenderer>();
    public int nSampleSphere = 50;
    public int nSampleCylinderH = 8;
    public int nSampleCylinderR = 15;
    public int nSampleSheet = 50;
    public int poolScalingFactor = 5; 

    [SerializeField]  public GameObject sphereCharge;
    [SerializeField]  public GameObject cylinderCharge;
    [SerializeField]  public GameObject sheetCharge_Face1;
    [SerializeField] public GameObject sheetCharge_Face2;
    //[SerializeField] public GameObject testObject;
    [SerializeField] public GameObject arrowHead;
    [SerializeField] public LineRenderer baseEFLine;
    [SerializeField] public GameObject pool;

    public float HEIGHT_ABOVE_SURFACE = 5f;


    //Object pooling - keeping it for later use when we might want to use a slider to adjust the charge magnitude and field lines
    private Queue<LineRenderer> linePool = new Queue<LineRenderer>();
    private List<LineRenderer> activeLineRenderers = new List<LineRenderer>();

    public UnityEngine.Vector3 pointChargePrevPos;
    public UnityEngine.Vector3 pointChargeCurrentPos; 


    private void Awake()
    {

        InitializeLinePool();
        SpreadSphereFieldLines();
        SpreadCylinderFieldLines();
        SpreadPlaneFieldLines(sheetCharge_Face1);
        SpreadPlaneFieldLinesOppositeFace(sheetCharge_Face2);


    }

    private int maxLines; //make sure it is significantly greater than combined nSampleSphere + nSampleCylinder + nSamplePlane
    private void InitializeLinePool()
    {
        maxLines = poolScalingFactor * (nSampleCylinderH + nSampleSphere + nSampleSheet);
        for (int i = 0; i < maxLines; i++)
        {
            LineRenderer lr = Instantiate(baseEFLine, pool.transform);
            lr.gameObject.SetActive(false); // Initially deactivate
            linePool.Enqueue(lr); // Add to the queue
            lr.gameObject.SetActive(false); // Initially deactivate
            linePool.Enqueue(lr); // Add to the queue
        }
    }


    void Start()
    {
        pointChargePrevPos = pointCharge.transform.position;

        //Setting position of guassian surface = position of the charge
        //sphereGaussianSurface.transform.position = pointCharge.transform.position;


    }

    void Update()
    {
        if (!pointCharge.GetComponent<Renderer>().isVisible) {
            for (int i = 0; i < activeLineRenderers.Count; i++) {
                activeLineRenderers[i].enabled = false;
            }
        }
        else
        {
            foreach (LineRenderer r in activeLineRenderers)
            {
                r.enabled = true;
            }
        }

        pointChargeCurrentPos = pointCharge.transform.position;

        if (pointChargeCurrentPos != pointChargePrevPos)
        {
            for (int i = 0; i < activeLineRenderers.Count; i++)
            {
                LineRenderer lr = activeLineRenderers[i];
                lr.gameObject.SetActive(false);
                lr.SetPosition(0, UnityEngine.Vector3.zero);
                lr.SetPosition(1, UnityEngine.Vector3.zero);
                lr.useWorldSpace = true;
                ReturnLineRendererToPool(lr);
            }

        }

        pointChargePrevPos = pointCharge.transform.position;
    }

    // Update is called once per frame

    
    void SphereFieldLines()
    {

        float phi = Mathf.PI * (Mathf.Sqrt(5) - 1);  // Golden angle approximation
        float radius = pointCharge.GetComponent<SphereCollider>().radius *
            Mathf.Max(pointCharge.transform.localScale.x, pointCharge.transform.localScale.y, pointCharge.transform.localScale.z);
        //Vector3 radius = pointCharge.GetComponent<Renderer>().bounds.extents;
        UnityEngine.Vector3 sphereCenter = transform.position;
        //Debug.Log("Sphere Center: " + sphereCenter);
        //Debug.Log("Radius:" + radius);

        for (int i = 0; i < nSampleSphere; i++)
        {
            float y = 1 - (i / (float)(nSampleSphere - 1)) * 2;  // Normalize y to be between -1 and 1
            float radiusAtY = Mathf.Sqrt(1 - y * y);  // Radius at height y

            float theta = phi * i;  // Incremental angle

            float x = Mathf.Cos(theta) * radiusAtY;
            float z = Mathf.Sin(theta) * radiusAtY;

            UnityEngine.Vector3 point_start = new UnityEngine.Vector3(x, y, z) * radius + sphereCenter;
            UnityEngine.Vector3 point_end = new UnityEngine.Vector3(x, y, z) * radius * 2 + sphereCenter;

            // Create LineRenderer
            GameObject lineObject = new GameObject("lr" + (fieldLines.Count + 1));
            LineRenderer newLine = lineObject.AddComponent<LineRenderer>();
            newLine.transform.SetParent(sphereCharge.transform, true);
            newLine.positionCount = 2;
            newLine.material = new Material(Shader.Find("Sprites/Default"));
            newLine.startColor = Color.blue;
            newLine.endColor = Color.blue;
            newLine.startWidth = 0.05f;
            newLine.endWidth = 0.05f;

            newLine.SetPosition(0, point_start);
            newLine.SetPosition(1, point_end);

            fieldLines.Add(newLine);

            //Debug.Log(newLine.GetPosition(1));
        }

    }

    List<Dictionary<string, UnityEngine.Vector3>> GenerateSphereFieldLines()
    {
        List<Dictionary<string, UnityEngine.Vector3>> fieldLines = new List<Dictionary<string, UnityEngine.Vector3>>();

        float phi = Mathf.PI * (Mathf.Sqrt(5) - 1);  // Golden angle approximation
        float radius = pointCharge.GetComponent<SphereCollider>().radius *
            Mathf.Max(pointCharge.transform.localScale.x, pointCharge.transform.localScale.y, pointCharge.transform.localScale.z);
        //UnityEngine.Vector3 radius = pointCharge.GetComponent<Renderer>().bounds.extents;
        UnityEngine.Vector3 sphereCenter = sphereCharge.transform.position;
        //Debug.Log("Sphere Center: " + sphereCenter);
        //Debug.Log("Radius:" + radius);

        for (int i = 0; i < nSampleSphere; i++)
        {
            float y = 1 - (i / (float)(nSampleSphere - 1)) * 2;  // Normalize y to be between -1 and 1
            float radiusAtY = Mathf.Sqrt(1 - y * y);  // Radius at height y

            float theta = phi * i;  // Incremental angle

            float x = Mathf.Cos(theta) * radiusAtY;
            float z = Mathf.Sin(theta) * radiusAtY;

            UnityEngine.Vector3 point_start = new UnityEngine.Vector3(x, y, z) * radius + sphereCenter;
            UnityEngine.Vector3 point_end = new UnityEngine.Vector3(x, y, z) * radius * 2 + sphereCenter;

            Dictionary<string, UnityEngine.Vector3> points = new Dictionary<string, UnityEngine.Vector3>();
            points.Add("start", point_start);
            points.Add("end", point_end);

            fieldLines.Add(points);
        }

        return fieldLines;
    }

    void SpreadSphereFieldLines()
    {
        List<Dictionary<string, UnityEngine.Vector3>> fLines = GenerateSphereFieldLines();

        for (int i = 0; i < fLines.Count; i++)
        {

            //GameObject lineObject = new GameObject("Sphere Field " + (i + 1));
            LineRenderer newLine = Instantiate(baseEFLine);
            //LineRenderer newLine = lineObject.AddComponent<LineRenderer>(); // change this line for pooling

            //LineRenderer newLine = GetLineRendererFromPool();
            newLine.gameObject.SetActive(true);
            Vector3 start = fLines[i]["start"];
            Vector3 end = fLines[i]["end"];
            newLine.SetPosition(0, fLines[i]["start"]);
            newLine.SetPosition(1, fLines[i]["end"]);

            newLine.transform.SetParent(sphereCharge.transform, true);
            newLine.positionCount = 2;

            newLine.material = new Material(Shader.Find("Sprites/Default"));
            newLine.startColor = Color.blue;
            newLine.endColor = Color.blue;
            newLine.startWidth = 0.05f;
            newLine.endWidth = 0.05f;

            GameObject arrow = Instantiate(arrowHead);
            arrow.transform.position = (start + end) / 2;
            arrow.transform.SetParent(newLine.transform);
            arrow.transform.up = (end - start).normalized;

        }
    }


    void SpreadCylinderFieldLines()
    {
        List<Dictionary<string, UnityEngine.Vector3>> fLines = GenerateCylinderFieldLines();

        for (int i = 0; i < fLines.Count; i++)
        {
            //GameObject test = Instantiate(testObject);
            //test.name = "test " + i;
            //test.transform.position = fLines[i]["surface"];

            //GameObject lineObject = new GameObject("Cylinder Field " + (i + 1));
            //LineRenderer newLine = lineObject.AddComponent<LineRenderer>(); // change this line for pooling

            LineRenderer newLine = Instantiate(baseEFLine);

            newLine.gameObject.SetActive(true);
            Vector3 start = fLines[i]["surface"];
            Vector3 end = fLines[i]["axis"];
            newLine.SetPosition(0, start);
            newLine.SetPosition(1, end);
            newLine.transform.SetParent(cylinderCharge.transform, true);
            newLine.positionCount = 2;
            newLine.material = new Material(Shader.Find("Sprites/Default"));
            newLine.startColor = Color.blue;
            newLine.endColor = Color.blue;
            newLine.startWidth = 0.05f;
            newLine.endWidth = 0.05f;

            GameObject arrow = Instantiate(arrowHead);
            arrow.transform.up = (end - start).normalized;
            arrow.transform.SetParent(newLine.transform);

            //if (!isPlayOn)
            //{
            arrow.transform.position = (start + end) / 2;
            //}
            //else
            //{
            //    float timeFactor = Mathf.PingPong(Time.time, 1); // Adjust the time factor as needed
            //    arrow.transform.position = Vector3.Lerp(start, end, timeFactor);
            //}


        }
    }

    List<Dictionary<string, UnityEngine.Vector3>> GenerateCylinderFieldLines()
    {
        List<Dictionary<string, UnityEngine.Vector3>> fieldLines = new List<Dictionary<string, UnityEngine.Vector3>>();

        float phi = Mathf.PI * (Mathf.Sqrt(5) - 1);  // Golden angle approximation
        float radius = cylinderCharge.GetComponent<CapsuleCollider>().radius *
            Mathf.Max(cylinderCharge.transform.localScale.x, cylinderCharge.transform.localScale.z);
        float height = cylinderCharge.GetComponent<CapsuleCollider>().height *
            cylinderCharge.transform.localScale.y;
        UnityEngine.Vector3 cylinderCenter = cylinderCharge.transform.position;
        float baseHeight = cylinderCenter.y - height / 2;
        //Debug.Log("Cylinder Center: " + cylinderCenter);
        //Debug.Log("Radius: " + radius);
        //Debug.Log("Height: " + height);
        //Debug.Log("Base Height: " + baseHeight);


        for (int i = 0; i < nSampleCylinderH; i++)
        {
            float y = baseHeight + height * i / nSampleCylinderH;
            UnityEngine.Vector3 center_offset = new UnityEngine.Vector3(cylinderCenter.x, y, cylinderCenter.z);

            for (int j = 0; j < nSampleCylinderR; j++)
            {
                float theta = phi * j + Random.Range(0, Mathf.PI/ nSampleCylinderR);

                float x = Mathf.Cos(theta) * radius;
                float z = Mathf.Sin(theta) * radius;

                UnityEngine.Vector3 pt_start = new UnityEngine.Vector3(x, 0, z) * radius + center_offset;
                //Debug.Log("pt on srf: " + pt_start);

                // Closest point on the central axis of the cylinder
                //UnityEngine.Vector3 point_on_axis = new UnityEngine.Vector3(0, y, 0) + cylinderCenter;
                UnityEngine.Vector3 pt_end = new UnityEngine.Vector3(x, 0, z) * radius * 5 + center_offset;
                //Debug.Log("pt out srf: " + pt_end);


                Vector3 offset_start = pt_start - cylinderCenter;
                Vector3 offset_end = pt_end - cylinderCenter;

                Vector3 rot_offset_s = Quaternion.AngleAxis(90, Vector3.forward) * offset_start;
                Vector3 rot_offset_e = Quaternion.AngleAxis(90, Vector3.forward) * offset_end;

                pt_start = rot_offset_s + cylinderCenter;
                pt_end = rot_offset_e + cylinderCenter;


                Dictionary<string, UnityEngine.Vector3> points = new Dictionary<string, UnityEngine.Vector3>();

                points.Add("surface", pt_start);
                points.Add("axis", pt_end);

                fieldLines.Add(points);
            }


        }

        return fieldLines;
    }


    void SpreadPlaneFieldLines(GameObject face)
    {
        if (face == null)
        {
            Debug.LogError("Face is null");
            return;
        }

        List<Dictionary<string, UnityEngine.Vector3>> fLines = GenerateLinesFromPlane(face, (int)(nSampleSheet / 2), 5f);

        for (int i = 0; i < fLines.Count; i++)
        {
            //GameObject lineObject = new GameObject("Sheet Field " + (i + 1));
            //Debug.Log(lineObject);
            //Debug.Log("Created Plane Field Lines");
            //LineRenderer newLine = lineObject.AddComponent<LineRenderer>();

            LineRenderer newLine = Instantiate(baseEFLine);
            newLine.gameObject.SetActive(true);

            Vector3 start = fLines[i]["start"];
            Vector3 end = fLines[i]["end"];

            newLine.SetPosition(0, fLines[i]["start"]);
            newLine.SetPosition(1, fLines[i]["end"]);
            newLine.transform.SetParent(face.transform, true);
            newLine.positionCount = 2;
            newLine.material = new Material(Shader.Find("Sprites/Default"));
            newLine.startColor = Color.blue;
            newLine.endColor = Color.blue;
            newLine.startWidth = 0.05f;
            newLine.endWidth = 0.05f;
            //newLine.useWorldSpace = true;

            GameObject arrow = Instantiate(arrowHead);
            arrow.transform.position = (start + end) / 2;
            arrow.transform.SetParent(newLine.transform);
            arrow.transform.up = (end - start).normalized;
        }
    }

    void SpreadPlaneFieldLinesOppositeFace(GameObject face)
    {
        if (face == null)
        {
            Debug.LogError("Face is null");
            return;
        }

        List<Dictionary<string, UnityEngine.Vector3>> fLines = GenerateLinesFromPlane(face, (int)(nSampleSheet / 2), 5f);

        for (int i = 0; i < fLines.Count; i++)
        {
            //GameObject lineObject = new GameObject("Sheet Field " + (i + 1));
            //Debug.Log(lineObject);
            //Debug.Log("Created Plane Field Lines");
            //LineRenderer newLine = lineObject.AddComponent<LineRenderer>();

            LineRenderer newLine = Instantiate(baseEFLine);
            newLine.gameObject.SetActive(true);

            Vector3 start = fLines[i]["start"];
            Vector3 end = new UnityEngine.Vector3(fLines[i]["end"].x, fLines[i]["end"].y - (2 * HEIGHT_ABOVE_SURFACE), fLines[i]["end"].z);

            newLine.SetPosition(0, fLines[i]["start"]);
            newLine.SetPosition(1, end );
            newLine.transform.SetParent(face.transform, true);
            newLine.positionCount = 2;
            newLine.material = new Material(Shader.Find("Sprites/Default"));
            newLine.startColor = Color.blue;
            newLine.endColor = Color.blue;
            newLine.startWidth = 0.05f;
            newLine.endWidth = 0.05f;
            //newLine.useWorldSpace = true;

            GameObject arrow = Instantiate(arrowHead);
            arrow.transform.position = (start + end) / 2;
            arrow.transform.SetParent(newLine.transform);
            arrow.transform.up = (end - start).normalized;
        }
    }


    List<Dictionary<string, UnityEngine.Vector3>> GenerateLinesFromPlane(GameObject plane, int numberOfLines, float heightAboveSurface)
    {
        List<Dictionary<string, UnityEngine.Vector3>> linesList = new List<Dictionary<string, UnityEngine.Vector3>>();

        if (plane != null)
        {
            MeshFilter meshFilter = plane.GetComponent<MeshFilter>();
            if (meshFilter != null)
            {
                Mesh mesh = meshFilter.mesh;
                UnityEngine.Vector3[] vertices = mesh.vertices;

                // Transform vertices to world coordinates
                for (int i = 0; i < vertices.Length; i++)
                {
                    vertices[i] = plane.transform.TransformPoint(vertices[i]);
                }

                // Recalculate bounds
                mesh.RecalculateBounds();

                // Calculate the bounds of the plane
                UnityEngine.Vector3 min = vertices[0];
                UnityEngine.Vector3 max = vertices[0];
                foreach (UnityEngine.Vector3 vertex in vertices)
                {
                    min = UnityEngine.Vector3.Min(min, vertex);
                    max = UnityEngine.Vector3.Max(max, vertex);
                }

                // Calculate the step size based on the number of lines
                int rows = Mathf.CeilToInt(Mathf.Sqrt(numberOfLines));
                int columns = Mathf.CeilToInt((float)numberOfLines / rows);
                //float stepY = (max.y - min.y) / (rows - 1);
                float stepX = (max.x - min.x) / (rows - 1);
                float stepZ = (max.z - min.z) / (columns - 1);

                // Generate lines in a grid pattern
                for (int i = 0; i < rows; i++)
                {
                    for (int j = 0; j < columns; j++)
                    {
                        if (linesList.Count >= numberOfLines)
                            break;

                        UnityEngine.Vector3 startPosition = new UnityEngine.Vector3(min.x + i * stepX, min.y, min.z + j * stepZ);
                        UnityEngine.Vector3 endPosition = new UnityEngine.Vector3(startPosition.x, startPosition.y + heightAboveSurface, startPosition.z);

                        Dictionary<string, UnityEngine.Vector3> lineData = new Dictionary<string, UnityEngine.Vector3>();
                        lineData.Add("start", startPosition);
                        lineData.Add("end", endPosition);

                        linesList.Add(lineData);

                        Debug.Log("MAX AND MINS FOR THE VERTICES min:" + min + " max:" + max + " Plane's position:" + plane.transform.position);
                    }
                }
            }
        }

        return linesList;
    }


    private LineRenderer GetLineRendererFromPool()
    {
        if (linePool.Count > 0)
        {
            LineRenderer lr = linePool.Dequeue();
            lr.gameObject.SetActive(true);
            activeLineRenderers.Add(lr);
            return lr;
        }
        else
        {
            GameObject lineObject = new GameObject("lr" + (fieldLines.Count + 1));
            LineRenderer lr = lineObject.AddComponent<LineRenderer>(); lr.gameObject.SetActive(true);
            activeLineRenderers.Add(lr);
            return lr;
        }
    }

    private void ReturnLineRendererToPool(LineRenderer lr)
    {
        lr.gameObject.SetActive(false);
        linePool.Enqueue(lr);
        activeLineRenderers.Remove(lr);
    }

    public int CalculateElectricFieldLines(float minMagnitude, float maxMagnitude, int minLines, int maxLines)
    {
        // Example charge magnitude (you can set this value as needed)
        float chargeMagnitude = 5.0f; // Replace this with the actual charge magnitude

        // Ensure the charge magnitude is within the specified range
        if (chargeMagnitude < minMagnitude)
        {
            chargeMagnitude = minMagnitude;
        }
        else if (chargeMagnitude > maxMagnitude)
        {
            chargeMagnitude = maxMagnitude;
        }

        // Normalize the charge magnitude to a 0-1 range
        float normalizedMagnitude = (chargeMagnitude - minMagnitude) / (maxMagnitude - minMagnitude);

        // Calculate the number of electric field lines based on the normalized magnitude
        int numberOfLines = Mathf.RoundToInt(normalizedMagnitude * (maxLines - minLines) + minLines);

        return numberOfLines;
    }
}
