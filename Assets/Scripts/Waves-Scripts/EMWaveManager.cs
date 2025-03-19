using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EMWaveManager : MonoBehaviour
{
    [Header("Wave Properties")]
    public float amplitude = 1.0f;      // Maximum field strength
    public float wavelength = 5.0f;     // Distance between wave peaks
    public float frequency = 1.0f;      // Wave oscillation speed
    public int pointCount = 20;         // Number of field vectors to display
    public float waveLength = 10.0f;    // Total length of wave visualization

    [Header("Visualization")]
    public Material electricFieldMaterial;  // Material for E-field (typically red)
    public Material magneticFieldMaterial;  // Material for B-field (typically blue) 
    public Material propagationMaterial;    // Material for propagation arrow (typically green)
    public float arrowRelativeSize = 0.5f;
    public bool showPropagationArrow = true;

    [Header("Debug Options")]
    public bool debugMode = true;       // Enable/disable debug logging

    // Parent GameObjects for organization
    private GameObject waveContainer;
    private GameObject propagationArrowObj;

    // Lists to store our field components
    private List<GameObject> fieldPoints = new List<GameObject>();
    private List<Arrow> electricFieldArrows = new List<Arrow>();
    private List<Arrow> magneticFieldArrows = new List<Arrow>();
    private Arrow propagationArrow;

    // Wave animation variables
    private float phase = 0.0f;

    void Start()
    {
        try
        {
            // Create a container for better organization in the hierarchy
            waveContainer = new GameObject("EM Wave Points");
            waveContainer.transform.parent = this.transform;
            waveContainer.transform.localPosition = Vector3.zero;

            // Make sure we have our materials
            CheckMaterials();

            // Create the propagation direction arrow (optional)
            if (showPropagationArrow)
            {
                CreatePropagationArrow();
            }

            // Set up the field vectors
            CreateFieldPoints();

            if (debugMode)
                Debug.Log("EMWaveManager initialization complete");
        }
        catch (System.Exception e)
        {
            Debug.LogError("Error in EMWaveManager Start(): " + e.Message);
        }
    }

    void CheckMaterials()
    {
        if (electricFieldMaterial == null)
        {
            Debug.LogWarning("Electric field material not assigned, creating default red material");
            electricFieldMaterial = new Material(Shader.Find("Standard"));
            electricFieldMaterial.color = Color.red;
        }

        if (magneticFieldMaterial == null)
        {
            Debug.LogWarning("Magnetic field material not assigned, creating default blue material");
            magneticFieldMaterial = new Material(Shader.Find("Standard"));
            magneticFieldMaterial.color = Color.blue;
        }

        if (showPropagationArrow && propagationMaterial == null)
        {
            Debug.LogWarning("Propagation material not assigned, creating default green material");
            propagationMaterial = new Material(Shader.Find("Standard"));
            propagationMaterial.color = Color.green;
        }
    }

    void CreatePropagationArrow()
    {
        try
        {
            propagationArrowObj = new GameObject("Propagation Direction");
            propagationArrowObj.transform.parent = this.transform;
            propagationArrowObj.transform.localPosition = new Vector3(0, 0, -1.0f);

            if (debugMode)
                Debug.Log("Creating propagation arrow");

            propagationArrow = new Arrow(
                parent: propagationArrowObj,
                initialDirection: Vector3.forward,
                lengthOfTail: waveLength + 2.0f,
                relativeSize: arrowRelativeSize * 0.5f,
                headMaterial: propagationMaterial,
                tailMaterial: propagationMaterial
            );

            propagationArrow.Update();

            if (debugMode)
                Debug.Log("Propagation arrow created successfully");
        }
        catch (System.Exception e)
        {
            Debug.LogError("Error creating propagation arrow: " + e.Message);
        }
    }

    void Update()
    {
        try
        {
            // Update phase based on time and frequency
            phase += Time.deltaTime * frequency * 2 * Mathf.PI;

            // Update all field vectors
            UpdateFieldVectors();
        }
        catch (System.Exception e)
        {
            Debug.LogError("Error in EMWaveManager Update(): " + e.Message);
        }
    }

    void CreateFieldPoints()
    {
        try
        {
            // Clear any existing points if recreating
            foreach (var point in fieldPoints)
            {
                Destroy(point);
            }
            fieldPoints.Clear();
            electricFieldArrows.Clear();
            magneticFieldArrows.Clear();

            // Calculate spacing between points
            float spacing = waveLength / (pointCount - 1);

            if (debugMode)
                Debug.Log($"Creating {pointCount} field points with spacing {spacing}");

            // Create field points along the z-axis (propagation direction)
            for (int i = 0; i < pointCount; i++)
            {
                // Create a point GameObject to hold our arrows
                GameObject point = new GameObject($"FieldPoint_{i}");
                point.transform.parent = waveContainer.transform;
                point.transform.localPosition = new Vector3(0, 0, i * spacing);

                // Create child objects for each field component
                GameObject eFieldObj = new GameObject("E-Field");
                eFieldObj.transform.parent = point.transform;
                eFieldObj.transform.localPosition = Vector3.zero;

                GameObject bFieldObj = new GameObject("B-Field");
                bFieldObj.transform.parent = point.transform;
                bFieldObj.transform.localPosition = Vector3.zero;

                if (debugMode && i == 0)
                    Debug.Log("Creating first electric field arrow");

                // Create electric field arrow (points up/down along y-axis)
                Arrow electricArrow = new Arrow(
                    parent: eFieldObj,
                    initialDirection: Vector3.up,  // E-field points vertically (y-axis)
                    lengthOfTail: 0,  // Initial length, will be updated in UpdateFieldVectors
                    relativeSize: arrowRelativeSize,
                    headMaterial: electricFieldMaterial,
                    tailMaterial: electricFieldMaterial
                );

                if (debugMode && i == 0)
                    Debug.Log("Creating first magnetic field arrow");

                // Create magnetic field arrow (points left/right along x-axis)
                Arrow magneticArrow = new Arrow(
                    parent: bFieldObj,
                    initialDirection: Vector3.right,  // B-field points horizontally (x-axis)
                    lengthOfTail: 0,  // Initial length, will be updated in UpdateFieldVectors
                    relativeSize: arrowRelativeSize,
                    headMaterial: magneticFieldMaterial,
                    tailMaterial: magneticFieldMaterial
                );

                // Add objects to our lists
                fieldPoints.Add(point);
                electricFieldArrows.Add(electricArrow);
                magneticFieldArrows.Add(magneticArrow);

                if (debugMode && i == 0)
                    Debug.Log("First field point created successfully");
            }

            if (debugMode)
                Debug.Log($"Created {fieldPoints.Count} field points");
        }
        catch (System.Exception e)
        {
            Debug.LogError("Error creating field points: " + e.Message);
        }
    }

    void UpdateFieldVectors()
    {
        try
        {
            // Update all field points
            for (int i = 0; i < fieldPoints.Count; i++)
            {
                // Get the position along the propagation axis (z-axis)
                float z = fieldPoints[i].transform.localPosition.z;

                // Calculate the phase at this position
                float pointPhase = phase - (z / wavelength) * 2 * Mathf.PI;

                // Calculate electric field amplitude (oscillates along y-axis)
                float eFieldStrength = amplitude * Mathf.Sin(pointPhase);

                // Calculate magnetic field amplitude (oscillates along x-axis)
                float bFieldStrength = amplitude * Mathf.Sin(pointPhase);

                // Get the child objects that hold our arrows
                Transform eFieldTransform = fieldPoints[i].transform.GetChild(0);
                Transform bFieldTransform = fieldPoints[i].transform.GetChild(1);

                // Update electric field arrow (y-axis)
                eFieldTransform.rotation = Quaternion.identity;  // Reset rotation
                electricFieldArrows[i].SetTailLength(Mathf.Abs(eFieldStrength));

                // Set correct direction based on field sign
                if (eFieldStrength < 0)
                {
                    eFieldTransform.rotation = Quaternion.Euler(0, 0, 180);  // Point down
                }
                electricFieldArrows[i].Update();

                // Update magnetic field arrow (x-axis)
                bFieldTransform.rotation = Quaternion.Euler(0, 0, 90);  // Start pointing right
                magneticFieldArrows[i].SetTailLength(Mathf.Abs(bFieldStrength));

                // Set correct direction based on field sign
                if (bFieldStrength < 0)
                {
                    bFieldTransform.rotation = Quaternion.Euler(0, 0, 270);  // Point left instead
                }
                magneticFieldArrows[i].Update();
            }
        }
        catch (System.Exception e)
        {
            Debug.LogError("Error updating field vectors: " + e.Message);
        }
    }

    // Helper methods for runtime control

    public void SetFrequency(float newFrequency)
    {
        frequency = newFrequency;
    }

    public void SetAmplitude(float newAmplitude)
    {
        amplitude = newAmplitude;
    }

    public void SetWavelength(float newWavelength)
    {
        wavelength = newWavelength;
    }

    public void ResetWave()
    {
        phase = 0.0f;
        CreateFieldPoints();
    }

    void OnDestroy()
    {
        // Clean up
        if (waveContainer != null)
            Destroy(waveContainer);

        if (propagationArrowObj != null)
            Destroy(propagationArrowObj);
    }
}


