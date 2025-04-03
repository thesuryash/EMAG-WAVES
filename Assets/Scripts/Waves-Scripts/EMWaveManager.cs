using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI; // Required for UI elements

//Camera mainCamera = Camera.main;


public class EMWaveManager : MonoBehaviour
{
    [Header("Wave Properties")]
    [SerializeField]  public float amplitude = 4.0f;      // Maximum field strength
    [SerializeField]  public float wavelength = 10.0f;     // Distance between wave peaks
    [SerializeField]  public float frequency = 0.5f;      // Wave oscillation speed
    [SerializeField]  public int pointCountPerWave = 200;         // Number of field vectors to display
    [SerializeField] public int wavesPerAxis = 2;         // KEEEP THIS LOW OR I AM NOT SURE WHAT WILLL HAPPEN!!
    [SerializeField] public float waveLength = 100.0f;

    public int spaceBetweenWaves = 10;
    // Total length of wave visualization

    [Header("UI Controls")]
    [SerializeField]  public Slider amplitudeSlider;      // Reference to amplitude slider
    [SerializeField] public Slider wavelengthSlider;     // Reference to wavelength slider  
    [SerializeField] public Slider frequencySlider;      // Reference to frequency slider
    [SerializeField] public TMP_InputField wavelengthInput;
    [SerializeField] public TMP_InputField frequencyInput;
    [SerializeField] public TMP_InputField amplitudeInput;

    [Header("UI Value Ranges")]
    [SerializeField]  public float minAmplitude = 3f;   // Minimum amplitude value
    [SerializeField] public float maxAmplitude = 5f;   // Maximum amplitude value
    [SerializeField] public float minWavelength = 10.0f;  // Minimum wavelength value
    [SerializeField] public float maxWavelength = 20.0f; // Maximum wavelength value
    [SerializeField] public float minFrequency = 0.0f;   // Minimum frequency value
    [SerializeField] public float maxFrequency = 1.0f;   // Maximum frequency value

    [Header("Visualization")]
    [SerializeField] public Material electricFieldMaterial;  // Material for E-field
    [SerializeField] public Material magneticFieldMaterial;  // Material for B-field 
    [SerializeField] public Material propagationMaterial;    // Material for propagation arrow 
    [SerializeField] public float arrowRelativeSize = 0.5f;
    [SerializeField] public bool showPropagationArrow = true;

    [Header("Debug Options")]
    [SerializeField] public bool debugMode = true;       

   
    private GameObject waveContainer;
    private GameObject propagationArrowObj;

   
    private List<GameObject> fieldPoints = new List<GameObject>();
    private List<Arrow> electricFieldArrows = new List<Arrow>();
    private List<Arrow> magneticFieldArrows = new List<Arrow>();
    private Arrow propagationArrow;


    private float phase = 0.0f;

    void Start()
    {
        try
        {
            Camera MainCamera = Camera.main;

            //container creation
            waveContainer = new GameObject("EM Wave Points");
            waveContainer.transform.parent = this.transform;
            waveContainer.transform.localPosition = Vector3.zero;

            
            CheckMaterials();

            
            SetupUIControls();

     
            if (showPropagationArrow)
            {
                CreatePropagationArrow();
            }

            CreateFieldPoints();

            if (debugMode)
                Debug.Log("EMWaveManager initialization complete");
        }
        catch (System.Exception e)
        {
            Debug.LogError("Error in EMWaveManager Start(): " + e.Message);
        }

        wavelengthInput.text = wavelengthSlider.value.ToString("0.000");
        frequencyInput.text = frequencySlider.value.ToString("0.000");
        amplitudeInput.text = amplitudeSlider.value.ToString("0.000");

        wavelengthSlider.onValueChanged.AddListener(WavelengthListener);
        frequencySlider.onValueChanged.AddListener(FrequencyListener);
        amplitudeSlider.onValueChanged.AddListener (AmplitudeListener);


    }

    void WavelengthListener(float valiue)
    {
        wavelengthInput.text = valiue.ToString("0.000");
    }

    void FrequencyListener(float value)
    {
        frequencyInput.text = value.ToString("0.000");
    }

    void AmplitudeListener(float value)
    {
        amplitudeInput.text = value.ToString("0.000");
    }


    void SetupUIControls()
    {
        // amplitude slider
        if (amplitudeSlider != null)
        {
           
            amplitudeSlider.minValue = minAmplitude;
            amplitudeSlider.maxValue = maxAmplitude;
            amplitudeSlider.value = amplitude;

            amplitudeSlider.onValueChanged.AddListener(SetAmplitude);

            if (debugMode)
                Debug.Log("Amplitude slider configured");
        }
        else if (debugMode)
        {
            Debug.LogWarning("Amplitude slider reference is missing");
        }

        // wavelength slider
        if (wavelengthSlider != null)
        {
           
            wavelengthSlider.minValue = minWavelength;
            wavelengthSlider.maxValue = maxWavelength;
            wavelengthSlider.value = wavelength;

            wavelengthSlider.onValueChanged.AddListener(SetWavelength);

            if (debugMode)
                Debug.Log("Wavelength slider configured");
        }
        else if (debugMode)
        {
            Debug.LogWarning("Wavelength slider reference is missing");
        }

        // freq slider
        if (frequencySlider != null)
        {
      
            frequencySlider.minValue = minFrequency;
            frequencySlider.maxValue = maxFrequency;
            frequencySlider.value = frequency;

          
            frequencySlider.onValueChanged.AddListener(SetFrequency);

            if (debugMode)
                Debug.Log("Frequency slider configured");
        }
        else if (debugMode)
        {
            Debug.LogWarning("Frequency slider reference is missing");
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
         
            phase += GlobalVariables.DeltaTime * frequency * 2 * Mathf.PI;
            //phase += Time.deltaTime * frequency * 2 * Mathf.PI;

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
            foreach (var point in fieldPoints)
            {
                Destroy(point);
            }
            fieldPoints.Clear();
            electricFieldArrows.Clear();
            magneticFieldArrows.Clear();

            // Calculate spacing between points
            float spacing = waveLength / (pointCountPerWave - 1);
            float spaceBetweenWaves = waveLength / 5; // Ensure we only create 5 waves on each axis

            if (debugMode)
                Debug.Log($"Creating {pointCountPerWave} field points with spacing {spacing}");

            // Create field points along the Z-axis (propagation direction)
            for (int i = 0; i < pointCountPerWave; i++) // Z-axis
            {
                for (int j = 0; j < 3; j++) // Y-axis: Create 5 waves
                {
                    for (int k = 0; k < 3; k++) // X-axis: Create 5 waves
                    {
                        // Create a point GameObject to hold our arrows
                        GameObject point = new GameObject($"FieldPoint_{i}{j}{k}");
                        point.transform.parent = waveContainer.transform;
                        point.transform.localPosition = new Vector3(k * spaceBetweenWaves, j * spaceBetweenWaves, i * spacing);

                        // Create child objects for each field component
                        GameObject eFieldObj = new GameObject("E-Field");
                        eFieldObj.transform.parent = point.transform;
                        eFieldObj.transform.localPosition = Vector3.zero;

                        GameObject bFieldObj = new GameObject("B-Field");
                        bFieldObj.transform.parent = point.transform;
                        bFieldObj.transform.localPosition = Vector3.zero;

                        // Create electric field arrow (points up/down along Y-axis)
                        Arrow electricArrow = new Arrow(
                            parent: eFieldObj,
                            initialDirection: Vector3.up,  // E-field points vertically (Y-axis)
                            lengthOfTail: 0,  // Initial length, will be updated in UpdateFieldVectors
                            relativeSize: arrowRelativeSize,
                            headMaterial: electricFieldMaterial,
                            tailMaterial: electricFieldMaterial
                        );

                        // Create magnetic field arrow (points left/right along X-axis)
                        Arrow magneticArrow = new Arrow(
                            parent: bFieldObj,
                            initialDirection: Vector3.right,  // B-field points horizontally (X-axis)
                            lengthOfTail: 0,  // Initial tail length
                            relativeSize: arrowRelativeSize,
                            headMaterial: magneticFieldMaterial,
                            tailMaterial: magneticFieldMaterial
                        );

                        fieldPoints.Add(point);
                        electricFieldArrows.Add(electricArrow);
                        magneticFieldArrows.Add(magneticArrow);
                    }
                }
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
                // get the position along the propagation axis (z-axis)
                float z = fieldPoints[i].transform.localPosition.z;

                // calculate the phase at this position
                float pointPhase = phase - (z / wavelength) * 2 * Mathf.PI;

                // calculate electric field amplitude (oscillates along y-axis)
                float eFieldStrength = amplitude * Mathf.Sin(pointPhase);

                // calculate magnetic field amplitude (oscillates along x-axis)
                float bFieldStrength = amplitude * Mathf.Sin(pointPhase);

                // retrives child objects that hold our arrows
                Transform eFieldTransform = fieldPoints[i].transform.GetChild(0);
                Transform bFieldTransform = fieldPoints[i].transform.GetChild(1);

                // update electric field arrow (y-axis)
                eFieldTransform.rotation = Quaternion.identity;  // Reset rotation
                electricFieldArrows[i].SetTailLength(Mathf.Abs(eFieldStrength));

                // set correct direction based on field sign
                if (eFieldStrength < 0)
                {
                    eFieldTransform.rotation = Quaternion.Euler(0, 0, 180);  // Point down
                }
                electricFieldArrows[i].Update();

                // Update magnetic field arrow (x-axis)
                bFieldTransform.rotation = Quaternion.Euler(0, 0, 90);  // Start pointing right
                magneticFieldArrows[i].SetTailLength(Mathf.Abs(bFieldStrength));

                // correct direction based on field sign
                if (bFieldStrength < 0)
                {
                    bFieldTransform.rotation = Quaternion.Euler(0, 0, 270);  // point left instead
                }
                magneticFieldArrows[i].Update();
            }
        }
        catch (System.Exception e)
        {
            Debug.LogError("Error updating field vectors: " + e.Message);
        }
    }

    // Methods for UI control

    public void SetFrequency(float newFrequency)
    {
        frequency = newFrequency;

        // Update the slider value if it exists and doesn't match
        if (frequencySlider != null && Mathf.Abs(frequencySlider.value - newFrequency) > 0.01f)
        {
            frequencySlider.value = newFrequency;
            frequencyInput.text = newFrequency.ToString();

            if (debugMode)
                Debug.Log($"Frequency set to {newFrequency}");
        }


    }

    public void SetAmplitude(float newAmplitude)
    {
        amplitude = newAmplitude;

        // same thing
        if (amplitudeSlider != null && Mathf.Abs(amplitudeSlider.value - newAmplitude) > 0.01f)
        {
            amplitudeSlider.value = newAmplitude;
            amplitudeInput.text = amplitude.ToString();

            if (debugMode)
                Debug.Log($"Amplitude set to {newAmplitude}");
        }


    }

    public void SetWavelength(float newWavelength)
    {
        wavelength = newWavelength;

        // same thing
        if (wavelengthSlider != null && Mathf.Abs(wavelengthSlider.value - newWavelength) > 0.01f)
        {
            wavelengthSlider.value = newWavelength;
            wavelengthInput.text = wavelength.ToString();

            if (debugMode)
                Debug.Log($"Wavelength set to {newWavelength}");
        }


    }

    public void ResetWave()
    {
        phase = 0.0f;

        // Reset all sliders to default values
        if (amplitudeSlider != null)
            amplitudeSlider.value = 1.0f;

        if (wavelengthSlider != null)
            wavelengthSlider.value = 5.0f;

        if (frequencySlider != null)
            frequencySlider.value = 1.0f;


        CreateFieldPoints();

        if (debugMode)
            Debug.Log("Wave reset to default values");
    }

    void OnDestroy()
    {
        // remove listeners to prevent memory leaks
        if (amplitudeSlider != null)
            amplitudeSlider.onValueChanged.RemoveListener(SetAmplitude);

        if (wavelengthSlider != null)
            wavelengthSlider.onValueChanged.RemoveListener(SetWavelength);

        if (frequencySlider != null)
            frequencySlider.onValueChanged.RemoveListener(SetFrequency);

        // clean up
        if (waveContainer != null)
            Destroy(waveContainer);

        if (propagationArrowObj != null)
            Destroy(propagationArrowObj);
    }
}
