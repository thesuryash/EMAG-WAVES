//using System.Collections;
//using System.Collections.Generic;
//using System.IO;
//using UnityEditor;
//using UnityEngine;
//using UnityEngine.ProBuilder;





//public class Arrow
//{

//    /*private GameObject parent;*/
//    private GameObject _parent;
//    private GameObject _tail;
//    private GameObject _head;
//    private Vector3 _arrowheadDirection;
//    private float _currentTailLength;
//    private Vector3 _lastParentPosition;
//    private Quaternion lastParentRotation;
//    private Vector3 _lastParentScale;
//    private float _lastParentArea;
//    private float _relativeSize;

//    private Vector3 parentSize;


//    // Constructor
//    public Arrow(GameObject parent, Vector3 initialDirection = default, float lengthOfTail = 1, float relativeSize = 1f, Material headMaterial = null, Material tailMaterial = null)
//    {
//        this._parent = parent;

//        // --------------------------------------------
//        // Initialize Arrowhead Direction
//        this._arrowheadDirection = (initialDirection == default) ? this._parent.transform.eulerAngles : initialDirection;

//        // --------------------------------------------
//        // Locate Script Folder and Prefabs
//        string scriptFolderPath = Path.GetDirectoryName(RootPath);

//        string headPrefabPath = Path.Combine(scriptFolderPath, "headPrefab.prefab");
//        string tailPrefabPath = Path.Combine(scriptFolderPath, "tailPrefab.prefab");

//        // Load and Instantiate Prefabs
//        GameObject headPrefab = AssetDatabase.LoadAssetAtPath<GameObject>(headPrefabPath);
//        GameObject tailPrefab = AssetDatabase.LoadAssetAtPath<GameObject>(tailPrefabPath);

//        if (headPrefab == null || tailPrefab == null)
//        {
//            Debug.LogError("Failed to load one or both prefabs. Check the file names and paths.");
//            return;
//        }

//        GameObject head_ = GameObject.Instantiate(headPrefab);
//        GameObject tail_ = GameObject.Instantiate(tailPrefab);

//        // --------------------------------------------
//        // Set Parent-Child Hierarchy
//        head_.transform.SetParent(parent.transform);
//        tail_.transform.SetParent(parent.transform);

//        this._head = head_;
//        this._tail = tail_;

//        // --------------------------------------------
//        // Initialize Other Attributes
//        this._currentTailLength = lengthOfTail;
//        this._arrowheadDirection = initialDirection;
//        this._lastParentPosition = this._parent.transform.position;
//        this.lastParentRotation = this._parent.transform.rotation;
//        this._lastParentScale = this._parent.transform.localScale;
//        this._lastParentArea = 0;

//        // --------------------------------------------
//        // Normalize Arrow Size Relative to Parent
//        Vector3 parentSize;

//        try
//        {
//            parentSize = this._parent.GetComponent<Renderer>().bounds.size;
//        }
//        catch
//        {
//            Debug.Log("No bound present in the parent. Defaulting to the localScale of the parent transform.");
//            parentSize = this._parent.transform.localScale;
//        }

//        Vector3 headSize = _head.GetComponent<Renderer>().bounds.size;
//        Vector3 tailSize = _tail.GetComponent<Renderer>().bounds.size;

//        float normalizationFactor = Mathf.Min(
//            parentSize.x / (headSize.x + tailSize.x),
//            parentSize.y / (headSize.y + tailSize.y),
//            parentSize.z / (headSize.z + tailSize.z)
//        );

//        _head.transform.localScale *= normalizationFactor * relativeSize; // relativeSize is 1f by default.
//        _tail.transform.localScale *= normalizationFactor * relativeSize;

//        // --------------------------------------------
//        // Setting the material

//        if (headMaterial != null)
//        {
//            this._head.GetComponent<Renderer>().material = headMaterial;
//        }

//        if(tailMaterial != null)
//        {
//            this._tail.GetComponent<Renderer>().material = tailMaterial;
//        }

//        // --------------------------------------------
//        // Final Log
//        Debug.Log("Arrow constructor called with prefabs loaded from script location.");
//    }

//    public static string RootPath
//    {
//        get
//        {
//            var g = AssetDatabase.FindAssets($"t:Script {nameof(Arrow)}");
//            return AssetDatabase.GUIDToAssetPath(g[0]);
//        }
//    }


//    public void SetParent(GameObject parent)
//    {
//        this._parent = parent;
//    }

//    public void SetInitialDirection(Vector3 direction)
//    {
//        this._arrowheadDirection = direction;
//    }
//    public void SetTailLength(float length)
//    {
//        if (length >= 0)
//        {
//            this._currentTailLength = length;
//            //Debug.Log("Tail Length: " + this._currentTailLength);

//        }

//    }


//    public void UpdateParentTransform()
//    {
//        this._lastParentPosition = this._parent.transform.position;
//        this.lastParentRotation = this._parent.transform.rotation;
//        this._lastParentScale = this._parent.transform.localScale;

//        //Debug.Log(this._lastParentPosition + "," + this.lastParentRotation);
//    }

//    public void SetParentArea(float area)
//    {
//        this._lastParentArea = area;
//    }

//    public bool IsParentTransformChanged()
//    {
//        if (this._parent != null)
//        {
//            return
//                    this._lastParentPosition != this._parent.transform.position ||
//                   this.lastParentRotation != this._parent.transform.rotation ||
//                   this._lastParentScale != this._parent.transform.localScale;
//        }
//        return false;
//    }


//    public void SetScene()
//    {
//        UpdateTail();
//        UpdateHead();
//        EnsureHeadIsEnabled();
//    }

//    public void Update()
//    {
//        UpdateHead();
//        UpdateTail();

//        EnsureHeadIsEnabled();
//    }

//    private void UpdateTail()
//    {
//        //Debug.Log(IsParentTransformChanged());
//        {
//            this._tail.transform.position = CalculateTailOffsetPosition();
//            this._tail.transform.rotation = this._parent.transform.rotation;

//            float x = this._tail.transform.localScale.x;
//            float z = this._tail.transform.localScale.z;
//            this._tail.transform.localScale = new Vector3(x, this._currentTailLength, z);

//            //Debug.Log("UpdateHead() done");
//        }
//    }


//    public static float CalculateLengthByValue(float value)
//    {
//        if (value > 0)
//        {//max area is 100 unit-squared and min is 1 unit-squared
//            float maxLength = 10f;
//            float length = (maxLength / 100) * value;

//            return length;
//        }
//        return 0;
//    }

//    private Vector3 CalculateTailOffsetPosition()
//    {
//        // Calculate half the Y scale
//        float halfYScale = this._tail.transform.localScale.y / 2;

//        // Get the direction vector from the tail's rotation
//        Vector3 direction = this._tail.transform.rotation * Vector3.up;

//        // Scale the direction vector by half the Y scale
//        Vector3 offset = direction * halfYScale;

//        // Calculate the new tail position
//        Vector3 tailPosition = this._parent.transform.position + offset;

//        return tailPosition;
//    }


//    public Vector3 CalculateHeadOffsetPosition()
//    {
//        float halfYScale = this._tail.transform.localScale.y / 2;

//        Vector3 direction = this._tail.transform.rotation * Vector3.up;

//        Vector3 tailCenterPosition = this._tail.transform.position;

//        Vector3 offset = direction * halfYScale;

//        Vector3 headPosition = this._tail.transform.position + offset;

//        return headPosition;

//    }

//    private void UpdateHead()
//    {
//        EnsureHeadIsEnabled();

//        // Get the bounds of the tail mesh
//        Bounds tailBounds = this._tail.GetComponent<MeshFilter>().mesh.bounds;
//        Vector3 tailEndWorldPosition = GetTailEndWorldPosition(tailBounds);

//        // Get the bounds of the parent object
//        Bounds parentBounds = this._parent.GetComponent<MeshFilter>().mesh.bounds;
//        Vector3 parentTopWorldPosition = GetParentTopWorldPosition(parentBounds);

//        // Calculate the offset to align the arrow's bottom with the parent's top
//        Vector3 arrowBottomWorldPosition = GetArrowBottomWorldPosition(tailBounds);
//        Vector3 offset = parentTopWorldPosition - arrowBottomWorldPosition;

//        // Position the head mesh with the calculated offset
//        /*this.head.transform.position = tailEndWorldPosition + offset;*/
//        this._head.transform.position = CalculateHeadOffsetPosition();
//        if (this._tail.transform.localScale.y < 0)
//        {
//            this._head.transform.rotation = Quaternion.AngleAxis(180, Vector3.up) * this._tail.transform.rotation;
//        }
//        else
//        {
//            this._head.transform.rotation = this._tail.transform.rotation;
//        }


//        //Debug.Log("UpdateHead() done");

//    }



//    private void EnsureHeadIsEnabled()
//    {
//        // Check if the local scale of the tail in the Y direction is zero
//        if (this._tail.transform.localScale.y == 0)
//        {
//            this._head.gameObject.SetActive(false);
//        }
//        else if (this._tail.transform.localScale.y != 0 /*&& this._head.gameObject.active == false*/)
//        {
//            this._head.gameObject.SetActive(true);
//        }
//    }


//    private Vector3 GetTailEndWorldPosition(Bounds tailBounds)
//    {
//        Vector3 tailEndLocalPosition = tailBounds.center + new Vector3(0, 0, tailBounds.extents.z);
//        return this._tail.transform.TransformPoint(tailEndLocalPosition);
//    }

//    private Vector3 GetParentTopWorldPosition(Bounds parentBounds)
//    {
//        Vector3 parentTopLocalPosition = parentBounds.center + new Vector3(0, parentBounds.extents.y, 0);
//        return this._parent.transform.TransformPoint(parentTopLocalPosition);
//    }

//    private Vector3 GetArrowBottomWorldPosition(Bounds tailBounds)
//    {
//        Vector3 arrowBottomLocalPosition = tailBounds.center - new Vector3(0, tailBounds.extents.y, 0);
//        return this._tail.transform.TransformPoint(arrowBottomLocalPosition);
//    }
//}


using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

public class Arrow
{
    private GameObject _parent;
    private GameObject _tail;
    private GameObject _head;
    private Vector3 _arrowheadDirection;
    private float _currentTailLength;
    private Vector3 _lastParentPosition;
    private Quaternion lastParentRotation;
    private Vector3 _lastParentScale;
    private float _lastParentArea;
    private float _relativeSize;

    private Vector3 parentSize;

    // Constructor
    public Arrow(GameObject parent, Vector3 initialDirection = default, float lengthOfTail = 1, float relativeSize = 1f, Material headMaterial = null, Material tailMaterial = null)
    {
        this._parent = parent;

        // --------------------------------------------
        // Initialize Arrowhead Direction
        this._arrowheadDirection = (initialDirection == default) ? this._parent.transform.eulerAngles : initialDirection;

        // --------------------------------------------
        // Locate Script Folder and Prefabs
        string scriptFolderPath = Path.GetDirectoryName(RootPath);
        Debug.Log("Arrow script folder path: " + scriptFolderPath);

        string headPrefabPath = Path.Combine(scriptFolderPath, "headPrefab.prefab");
        string tailPrefabPath = Path.Combine(scriptFolderPath, "tailPrefab.prefab");

        Debug.Log("Looking for head prefab at: " + headPrefabPath);
        Debug.Log("Looking for tail prefab at: " + tailPrefabPath);

     
        GameObject headPrefab = null;
        GameObject tailPrefab = null;

        try
        {
            headPrefab = AssetDatabase.LoadAssetAtPath<GameObject>(headPrefabPath);
            tailPrefab = AssetDatabase.LoadAssetAtPath<GameObject>(tailPrefabPath);

            if (headPrefab == null)
            {
                Debug.LogError("Failed to load head prefab. Creating fallback geometry.");
                // Create a simple cone as fallback
                headPrefab = CreateFallbackHead();
            }

            if (tailPrefab == null)
            {
                Debug.LogError("Failed to load tail prefab. Creating fallback geometry.");
                // Create a simple cylinder as fallback
                tailPrefab = CreateFallbackTail();
            }
        }
        catch (System.Exception e)
        {
            Debug.LogError("Error loading prefabs: " + e.Message);
            // Create fallback geometry
            headPrefab = CreateFallbackHead();
            tailPrefab = CreateFallbackTail();
        }

        GameObject head_ = GameObject.Instantiate(headPrefab);
        GameObject tail_ = GameObject.Instantiate(tailPrefab);

        // --------------------------------------------
        // Set Parent-Child Hierarchy
        head_.transform.SetParent(parent.transform);
        tail_.transform.SetParent(parent.transform);

        this._head = head_;
        this._tail = tail_;

        // --------------------------------------------
        // Initialize Other Attributes
        this._currentTailLength = lengthOfTail;
        this._arrowheadDirection = initialDirection;
        this._lastParentPosition = this._parent.transform.position;
        this.lastParentRotation = this._parent.transform.rotation;
        this._lastParentScale = this._parent.transform.localScale;
        this._lastParentArea = 0;

        // --------------------------------------------
        // Normalize Arrow Size Relative to Parent
        try
        {
            if (this._parent.GetComponent<Renderer>() != null)
            {
                parentSize = this._parent.GetComponent<Renderer>().bounds.size;
            }
            else
            {
                Debug.Log("No Renderer found on parent. Using transform scale.");
                parentSize = this._parent.transform.localScale;
            }
        }
        catch
        {
            Debug.Log("Error getting parent size. Defaulting to transform scale.");
            parentSize = this._parent.transform.localScale;
        }

        Vector3 headSize = Vector3.one;
        Vector3 tailSize = Vector3.one;

        try
        {
            if (_head.GetComponent<Renderer>() != null)
                headSize = _head.GetComponent<Renderer>().bounds.size;

            if (_tail.GetComponent<Renderer>() != null)
                tailSize = _tail.GetComponent<Renderer>().bounds.size;
        }
        catch (System.Exception e)
        {
            Debug.LogError("Error getting arrow component size: " + e.Message);
        }

        float normalizationFactor = 0.2f; // Default value if calculation fails

        try
        {
            // Avoid division by zero
            if (headSize.x + tailSize.x > 0 && headSize.y + tailSize.y > 0 && headSize.z + tailSize.z > 0)
            {
                normalizationFactor = Mathf.Min(
                    parentSize.x / (headSize.x + tailSize.x),
                    parentSize.y / (headSize.y + tailSize.y),
                    parentSize.z / (headSize.z + tailSize.z)
                );
            }
        }
        catch (System.Exception e)
        {
            Debug.LogError("Error in normalization calculation: " + e.Message);
        }

        // Apply scaling
        try
        {
            _head.transform.localScale *= normalizationFactor * relativeSize;
            _tail.transform.localScale *= normalizationFactor * relativeSize;
        }
        catch (System.Exception e)
        {
            Debug.LogError("Error applying scale: " + e.Message);
        }

        // --------------------------------------------
        // Setting the material
        try
        {
            if (headMaterial != null && _head.GetComponent<Renderer>() != null)
            {
                this._head.GetComponent<Renderer>().material = headMaterial;
            }

            if (tailMaterial != null && _tail.GetComponent<Renderer>() != null)
            {
                this._tail.GetComponent<Renderer>().material = tailMaterial;
            }
        }
        catch (System.Exception e)
        {
            Debug.LogError("Error applying materials: " + e.Message);
        }

        // --------------------------------------------
        Debug.Log("Arrow constructor completed successfully.");
    }
    //these are geometries for head and tail creation fallback

    // Create fallback cone for head
    private GameObject CreateFallbackHead()
    {
        GameObject cone = new GameObject("FallbackArrowHead");
        MeshFilter mf = cone.AddComponent<MeshFilter>();
        MeshRenderer mr = cone.AddComponent<MeshRenderer>();

        // Create a simple cone mesh
        Mesh coneMesh = new Mesh();

        // Simple cone vertices
        Vector3[] vertices = new Vector3[5];
        vertices[0] = new Vector3(0, 0.5f, 0); // Tip
        vertices[1] = new Vector3(0.1f, 0, 0.1f); // Base
        vertices[2] = new Vector3(0.1f, 0, -0.1f);
        vertices[3] = new Vector3(-0.1f, 0, -0.1f);
        vertices[4] = new Vector3(-0.1f, 0, 0.1f);

        // Triangles (indices)
        int[] triangles = new int[12];
        // Side faces
        triangles[0] = 0; triangles[1] = 1; triangles[2] = 2;
        triangles[3] = 0; triangles[4] = 2; triangles[5] = 3;
        triangles[6] = 0; triangles[7] = 3; triangles[8] = 4;
        triangles[9] = 0; triangles[10] = 4; triangles[11] = 1;

        coneMesh.vertices = vertices;
        coneMesh.triangles = triangles;
        coneMesh.RecalculateNormals();

        mf.mesh = coneMesh;
        mr.material = new Material(Shader.Find("Standard"));

        return cone;
    }

    // Create fallback cylinder for tail
    private GameObject CreateFallbackTail()
    {
        GameObject cylinder = new GameObject("FallbackArrowTail");
        MeshFilter mf = cylinder.AddComponent<MeshFilter>();
        MeshRenderer mr = cylinder.AddComponent<MeshRenderer>();

        // Create a simple cylinder mesh
        Mesh cylinderMesh = new Mesh();

        // Simple cylinder with 4 sides
        Vector3[] vertices = new Vector3[8];
        // Top face
        vertices[0] = new Vector3(0.02f, 0.5f, 0.02f);
        vertices[1] = new Vector3(0.02f, 0.5f, -0.02f);
        vertices[2] = new Vector3(-0.02f, 0.5f, -0.02f);
        vertices[3] = new Vector3(-0.02f, 0.5f, 0.02f);
        // Bottom face
        vertices[4] = new Vector3(0.02f, -0.5f, 0.02f);
        vertices[5] = new Vector3(0.02f, -0.5f, -0.02f);
        vertices[6] = new Vector3(-0.02f, -0.5f, -0.02f);
        vertices[7] = new Vector3(-0.02f, -0.5f, 0.02f);

        // Triangles (indices)
        int[] triangles = new int[24];
        // Side faces
        triangles[0] = 0; triangles[1] = 1; triangles[2] = 5;
        triangles[3] = 0; triangles[4] = 5; triangles[5] = 4;
        triangles[6] = 1; triangles[7] = 2; triangles[8] = 6;
        triangles[9] = 1; triangles[10] = 6; triangles[11] = 5;
        triangles[12] = 2; triangles[13] = 3; triangles[14] = 7;
        triangles[15] = 2; triangles[16] = 7; triangles[17] = 6;
        triangles[18] = 3; triangles[19] = 0; triangles[20] = 4;
        triangles[21] = 3; triangles[22] = 4; triangles[23] = 7;

        cylinderMesh.vertices = vertices;
        cylinderMesh.triangles = triangles;
        cylinderMesh.RecalculateNormals();

        mf.mesh = cylinderMesh;
        mr.material = new Material(Shader.Find("Standard"));

        return cylinder;
    }

    public static string RootPath
    {
        get
        {
            var g = AssetDatabase.FindAssets($"t:Script {nameof(Arrow)}");
            if (g.Length == 0)
            {
                Debug.LogError("Cannot find Arrow script in project!");
                return "Assets";
            }
            string path = AssetDatabase.GUIDToAssetPath(g[0]);
            Debug.Log("Found Arrow script at: " + path);
            return path;
        }
    }


    public void SetParent(GameObject parent)
    {
        this._parent = parent;
    }

    public void SetInitialDirection(Vector3 direction)
    {
        this._arrowheadDirection = direction;
    }

    public void SetTailLength(float length)
    {
        if (length >= 0)
        {
            this._currentTailLength = length;
        }
    }

    public void UpdateParentTransform()
    {
        this._lastParentPosition = this._parent.transform.position;
        this.lastParentRotation = this._parent.transform.rotation;
        this._lastParentScale = this._parent.transform.localScale;
    }

    public void SetParentArea(float area)
    {
        this._lastParentArea = area;
    }

    public bool IsParentTransformChanged()
    {
        if (this._parent != null)
        {
            return
                this._lastParentPosition != this._parent.transform.position ||
                this.lastParentRotation != this._parent.transform.rotation ||
                this._lastParentScale != this._parent.transform.localScale;
        }
        return false;
    }

    public void SetScene()
    {
        UpdateTail();
        UpdateHead();
        EnsureHeadIsEnabled();
    }

    public void Update()
    {
        try
        {
            UpdateHead();
            UpdateTail();
            EnsureHeadIsEnabled();
        }
        catch (System.Exception e)
        {
            Debug.LogError("Error in Arrow.Update(): " + e.Message);
        }
    }

    private void UpdateTail()
    {
        try
        {
            this._tail.transform.position = CalculateTailOffsetPosition();
            this._tail.transform.rotation = this._parent.transform.rotation;

            float x = this._tail.transform.localScale.x;
            float z = this._tail.transform.localScale.z;
            this._tail.transform.localScale = new Vector3(x, this._currentTailLength, z);
        }
        catch (System.Exception e)
        {
            Debug.LogError("Error in UpdateTail(): " + e.Message);
        }
    }

    public static float CalculateLengthByValue(float value)
    {
        if (value > 0)
        {//max area is 100 unit-squared and min is 1 unit-squared
            float maxLength = 10f;
            float length = (maxLength / 100) * value;

            return length;
        }
        return 0;
    }

    private Vector3 CalculateTailOffsetPosition()
    {
        try
        {
            // Calculate half the Y scale
            float halfYScale = this._tail.transform.localScale.y / 2;

            // Get the direction vector from the tail's rotation
            Vector3 direction = this._tail.transform.rotation * Vector3.up;

            // Scale the direction vector by half the Y scale
            Vector3 offset = direction * halfYScale;

            // Calculate the new tail position
            Vector3 tailPosition = this._parent.transform.position + offset;

            return tailPosition;
        }
        catch (System.Exception e)
        {
            Debug.LogError("Error calculating tail position: " + e.Message);
            return this._parent.transform.position;
        }
    }

    public Vector3 CalculateHeadOffsetPosition()
    {
        try
        {
            float halfYScale = this._tail.transform.localScale.y / 2;

            Vector3 direction = this._tail.transform.rotation * Vector3.up;

            Vector3 tailCenterPosition = this._tail.transform.position;

            Vector3 offset = direction * halfYScale;

            Vector3 headPosition = this._tail.transform.position + offset;

            return headPosition;
        }
        catch (System.Exception e)
        {
            Debug.LogError("Error calculating head position: " + e.Message);
            return this._parent.transform.position;
        }
    }

    private void UpdateHead()
    {
        try
        {
            EnsureHeadIsEnabled();

            this._head.transform.position = CalculateHeadOffsetPosition();

            if (this._tail.transform.localScale.y < 0)
            {
                this._head.transform.rotation = Quaternion.AngleAxis(180, Vector3.up) * this._tail.transform.rotation;
            }
            else
            {
                this._head.transform.rotation = this._tail.transform.rotation;
            }
        }
        catch (System.Exception e)
        {
            Debug.LogError("Error in UpdateHead(): " + e.Message);
        }
    }

    private void EnsureHeadIsEnabled()
    {
        try
        {
            // Check if the local scale of the tail in the Y direction is zero
            if (this._tail.transform.localScale.y == 0)
            {
                this._head.gameObject.SetActive(false);
            }
            else if (this._tail.transform.localScale.y != 0)
            {
                this._head.gameObject.SetActive(true);
            }
        }
        catch (System.Exception e)
        {
            Debug.LogError("Error in EnsureHeadIsEnabled(): " + e.Message);
        }
    }

    private Vector3 GetTailEndWorldPosition(Bounds tailBounds)
    {
        Vector3 tailEndLocalPosition = tailBounds.center + new Vector3(0, 0, tailBounds.extents.z);
        return this._tail.transform.TransformPoint(tailEndLocalPosition);
    }

    private Vector3 GetParentTopWorldPosition(Bounds parentBounds)
    {
        Vector3 parentTopLocalPosition = parentBounds.center + new Vector3(0, parentBounds.extents.y, 0);
        return this._parent.transform.TransformPoint(parentTopLocalPosition);
    }

    private Vector3 GetArrowBottomWorldPosition(Bounds tailBounds)
    {
        Vector3 arrowBottomLocalPosition = tailBounds.center - new Vector3(0, tailBounds.extents.y, 0);
        return this._tail.transform.TransformPoint(arrowBottomLocalPosition);
    }
}

