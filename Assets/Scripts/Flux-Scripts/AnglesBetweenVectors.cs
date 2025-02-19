    
using System.Collections.Generic;
using UnityEngine;

public class AngleBetweenVectors
{
    private Vector3 _vector1;
    private Vector3 _vector2;
    private float _radiusOfAngle;

    private Vector3 center;

    public AngleBetweenVectors(Vector3 v1, Vector3 v2, float radiusOfAngle)
    {
        this._vector1 = v1;
        this._vector2 = v2;
        this._radiusOfAngle = radiusOfAngle;
    }

    public void ShowAngleSector()
    {
        GameObject meshObject = new GameObject("Angle");
        MeshFilter meshFilter = meshObject.AddComponent<MeshFilter>();
        meshFilter.mesh = ChooseSector();

        MeshRenderer meshRenderer = meshObject.AddComponent<MeshRenderer>();
        meshRenderer.material = new Material(Shader.Find("Standard"));

        meshObject.transform.position = Vector3.zero;
        meshObject.transform.rotation = Quaternion.identity;
        meshObject.transform.localScale = Vector3.one;
    }

    public Vector4 CreatePlaneFromVectors()
    {
        // Compute the normal vector
        Vector3 normal = Vector3.Cross(this._vector1, this._vector2);
        normal.Normalize(); // Normalize the vector

        // Assuming v1 is a point on the plane
        float D = -Vector3.Dot(normal, this._vector1);

        // Return the plane as a Vector4
        return new Vector4(normal.x, normal.y, normal.z, D);
    }

    public List<Mesh> CreateSectors()
    {
        List<Mesh> sectors = new List<Mesh>();
        Vector3 center = Vector3.zero;  // Origin
        int resolution = 100;           // Number of segments in the arc

        // Create each sector as a separate mesh
        sectors.Add(CreateSectorMesh(_vector1, _vector2, resolution));
        sectors.Add(CreateSectorMesh(_vector2, _vector1, resolution));

        return sectors;
    }

    private Mesh CreateSectorMesh(Vector3 startVec, Vector3 endVec, int resolution)
    {
        Mesh mesh = new Mesh();
        List<Vector3> vertices = new List<Vector3>();
        List<int> triangles = new List<int>();



        Vector3 normal = Vector3.Cross(startVec, endVec).normalized;
        vertices.Add(center); // add the center point

        for (int i = 0; i <= resolution; i++)
        {
            float t = i / (float)resolution;
            Vector3 spherical = Vector3.Slerp(startVec, endVec, t).normalized * _radiusOfAngle;
            vertices.Add(center + spherical);
        }

        for (int i = 1; i <= resolution; i++)
        {
            triangles.Add(0);
            triangles.Add(i);
            triangles.Add(i + 1);
        }

        mesh.vertices = vertices.ToArray();
        mesh.triangles = triangles.ToArray();
        mesh.RecalculateNormals(); // To ensure lighting calculates correctly

        return mesh;
    }

    public Mesh ChooseSector()
    {
        List<Mesh> sectors = new List<Mesh>();
        Dictionary<Mesh, float> sectorDict = new Dictionary<Mesh, float>();
       
        if (sectorDict.Count > 0) {
                Mesh smallerMesh = sectors[0];
                for (int i = 0; i < sectors.Count; i++)
                {
                    float smallArea = CalculateMeshArea(smallerMesh);
                    float currentArea = CalculateMeshArea(sectors[i]);

                    if (currentArea < smallArea)
                    {
                        smallerMesh = sectors[i];
                    }
                }
            return smallerMesh;


        }
        else return null;

    }

    public static float CalculateMeshArea(Mesh mesh)
    {
        float totalArea = 0f;
        Vector3[] vertices = mesh.vertices;
        int[] triangles = mesh.triangles;

        for (int i = 0; i < triangles.Length; i += 3)
        {
            Vector3 v0 = vertices[triangles[i]];
            Vector3 v1 = vertices[triangles[i + 1]];
            Vector3 v2 = vertices[triangles[i + 2]];

            // Calculate the area of the triangle formed by v0, v1, and v2
            float triangleArea = Vector3.Cross(v1 - v0, v2 - v0).magnitude * 0.5f;
            totalArea += triangleArea;
        }

        return totalArea;
    }
}




