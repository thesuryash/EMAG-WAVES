using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI; // Required for UI elements

//Camera mainCamera = Camera.main;


public class EMWaveManager : MonoBehaviour
{
    [Header("Wave Properties")]
    [SerializeField] public float amplitude = 3f;      // Maximum field strength
    [SerializeField] public float wavelength = 10f;     // Distance between wave peaks
    [SerializeField] public float frequency;      // Wave oscillation speed
    [SerializeField] public int pointCountPerWave = 200;         // Number of field vectors to display
    [SerializeField] public int wavesPerAxis = 2;         // KEEEP THIS LOW OR I AM NOT SURE WHAT WILLL HAPPEN!!
    [SerializeField] public float waveLength = 100.0f;
    [SerializeField] public int waveRows = 1; //this controls how many waves in parallel ?

    public float waveSpeed;
    public int spaceBetweenWaves = 10;
    // Total length of wave visualization

    [Header("Wave Speed")]
    [SerializeField] public bool keepWaveSpeedConstant = true;
    [SerializeField] public float constantWaveSpeed = 62.83f; // 2pi 10 * 1 (default values)

    [Header("UI Controls")]
    [SerializeField] public Slider amplitudeSlider;      // Reference to amplitude slider
    [SerializeField] public Slider wavelengthSlider;     // Reference to wavelength slider  
    [SerializeField] public Slider frequencySlider;      // Reference to frequency slider
    [SerializeField] public TMP_InputField wavelengthInput;
    [SerializeField] public TMP_InputField frequencyInput;
    [SerializeField] public TMP_InputField amplitudeInput;
    [SerializeField] public GameObject cameraTarget;

    [Header("UI Value Ranges")]
    [SerializeField] public float minAmplitude = 3f;   // Minimum amplitude value
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
    [SerializeField] public bool showPropagationArrow = false;

    [Header("Peak Visualization")]
    [SerializeField] public Color peakColor = new Color(0.7f, 0.1f, 0.1f); // Darker red
    [SerializeField][Range(0.7f, 1.0f)] public float peakThreshold = 0.9f;
    [SerializeField] public bool enablePeakColorChange = true;
    [Header("Debug Options")]
    [SerializeField] public bool debugMode = true;

    [Header("Optimization")]
    [SerializeField] private bool useObjectPooling = true;
    [SerializeField] private int initialPoolSize = 1000;
    private List<GameObject> fieldPointPool = new List<GameObject>();
    private List<Arrow> electricArrowPool = new List<Arrow>();
    private List<Arrow> magneticArrowPool = new List<Arrow>();

    [Header("Culling Settings")]
    [SerializeField] private bool useDistanceCulling = true;
    [SerializeField] private float maxViewDistance = 100f;
    [SerializeField] private float cullingUpdateInterval = 0.2f; // How often to check distances
    private float nextCullingUpdate = 0f;

    private GameObject waveContainer;
    private GameObject propagationArrowObj;

    private List<GameObject> fieldPoints = new List<GameObject>();
    private List<Arrow> electricFieldArrows = new List<Arrow>();
    private List<Arrow> magneticFieldArrows = new List<Arrow>();
    private Arrow propagationArrow;

    private float phase = 0.0f;

    private Vector3 avgPos;
    private int count;

    private void Awake()
    {

        try
        {
            Camera MainCamera = Camera.main;

            // Container creation
            waveContainer = new GameObject("EM Wave Points");
            waveContainer.transform.parent = this.transform;
            waveContainer.transform.localPosition = Vector3.zero;

            CheckMaterials();
            SetupUIControls();

            // Calculate initial wave speed
            constantWaveSpeed = 2 * Mathf.PI * wavelength * frequency;

            if (showPropagationArrow)
            {
                CreatePropagationArrow();
            }

            // Initialize object pools
            InitializeObjectPools();

            // Create field points
            CreateFieldPoints();

            cameraTarget.transform.position = avgPos;

            if (debugMode)
                Debug.Log("EMWaveManager initialization complete");

            if (useDistanceCulling)
            {
                nextCullingUpdate = Time.time + cullingUpdateInterval;
                UpdateFieldPointCulling();
            }
        }
        catch (System.Exception e)
        {
            Debug.LogError("Error in EMWaveManager Start(): " + e.Message);
        }

    }
    void Start()
    {
  

        waveSpeed = waveLength * frequency * 2 * Mathf.PI;
        wavelengthInput.text = wavelengthSlider.value.ToString("0.000");
        frequencyInput.text = frequencySlider.value.ToString("0.000");
        amplitudeInput.text = amplitudeSlider.value.ToString("0.000");

        wavelengthSlider.onValueChanged.AddListener(WavelengthListener);
        frequencySlider.onValueChanged.AddListener(FrequencyListener);
        amplitudeSlider.onValueChanged.AddListener(AmplitudeListener);

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
            amplitudeInput.onEndEdit.AddListener(onAmplitudeInputChanged);

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
            wavelengthInput.onEndEdit.AddListener(onWavelengthInputChanged);

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
            frequencySlider.interactable = false;

            frequencySlider.onValueChanged.AddListener(SetFrequency);
            frequencyInput.onEndEdit.AddListener(onFrequencyInputChanged);
            frequencyInput.interactable = false;

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

    void UpdateFieldPointCulling()
    {
        if (!useDistanceCulling) return;

        Camera mainCamera = Camera.main;
        if (mainCamera == null) return;

        Vector3 cameraPosition = mainCamera.transform.position;

        for (int i = 0; i < fieldPoints.Count; i++)
        {
            GameObject point = fieldPoints[i];
            float distanceToCamera = Vector3.Distance(point.transform.position, cameraPosition);

            // Only show points within the max view distance
            bool shouldBeVisible = distanceToCamera <= maxViewDistance;

            // Only change active state if it's different to avoid unnecessary SetActive calls
            if (point.activeSelf != shouldBeVisible)
            {
                point.SetActive(shouldBeVisible);
            }
        }
    }

    void CreatePropagationArrow()
    {
        try
        {
            propagationArrowObj = new GameObject("Propagation Direction");

            propagationArrowObj.SetActive(false);

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
            phase += GlobalVariables.DeltaTime * frequency;

            // Update field vectors
            UpdateFieldVectors();

            // Check if it's time to update culling
            if (useDistanceCulling && Time.time >= nextCullingUpdate)
            {
                UpdateFieldPointCulling();
                nextCullingUpdate = Time.time + cullingUpdateInterval;
            }
        }
        catch (System.Exception e)
        {
            Debug.LogError("Error in EMWaveManager Update(): " + e.Message);
        }

        cameraTarget.transform.position = avgPos;

    }

    void InitializeObjectPools()
    {
        if (!useObjectPooling) return;

        // Pre-create field points for the pool
        for (int i = 0; i < initialPoolSize; i++)
        {
            // Create a point GameObject
            GameObject point = new GameObject($"PooledFieldPoint_{i}");
            point.transform.parent = waveContainer.transform;
            point.SetActive(false);

            // Add to pools
            fieldPointPool.Add(point);
            electricFieldArrows.Add(CreateArrow("Electric", point));
            magneticFieldArrows.Add(CreateArrow("Magnetic", point));
        }

        if (debugMode)
            Debug.Log($"Initialized object pool with {initialPoolSize} objects");
    }

    GameObject GetFieldPointFromPool(Vector3 position)
    {
        if (!useObjectPooling)
        {
            // if not using pooling, we will create a new point
            return CreateNewFieldPoint(position);
        }

        // Look for inactive point in pool
        foreach (var point in fieldPointPool)
        {
            if (!point.activeInHierarchy)
            {
                point.transform.localPosition = position;
                point.SetActive(true);
                return point;
            }
        }

        // If no inactive points available, expand the pool
        if (debugMode)
            Debug.Log("Pool expanded, adding more objects");

        // Create a new point and add to pool
        GameObject newPoint = CreateNewFieldPoint(position);
        fieldPointPool.Add(newPoint);
        return newPoint;
    }
    //pooling-based approach
    private GameObject CreateNewFieldPoint(Vector3 position)
    {
        int index = fieldPointPool.Count;

        // create a point GameObject
        GameObject point = new GameObject($"PooledFieldPoint_{index}");
        point.transform.parent = waveContainer.transform;
        point.transform.localPosition = position;

        electricFieldArrows.Add(CreateArrow("Electric", point));
        magneticFieldArrows.Add(CreateArrow("Magnetic", point));

        return point;
    }

    void CreateFieldPoints()
    {
        try
        {
            // First, deactivate all currently active field points
            foreach (var point in fieldPoints)
            {
                point.SetActive(false);
            }

            fieldPoints.Clear();
            electricFieldArrows.Clear();
            magneticFieldArrows.Clear();

            // Calculate spacing between points
            float spacing = waveLength / (pointCountPerWave - 1);
            float spaceBetweenWaves = waveLength / 5;

            if (debugMode)
                Debug.Log($"Creating {pointCountPerWave * waveRows * waveRows} field points with spacing {spacing}");

            // Calculate the total number of points we'll need
            int totalPoints = pointCountPerWave * waveRows * waveRows;

            // Check if we need to expand the pool
            if (useObjectPooling && fieldPointPool.Count < totalPoints)
            {
                int additionalPointsNeeded = totalPoints - fieldPointPool.Count;
                if (debugMode)
                    Debug.Log($"Need to add {additionalPointsNeeded} more points to the pool");

                // Expand our pools if necessary
                for (int i = 0; i < additionalPointsNeeded; i++)
                {
                    Vector3 dummyPosition = Vector3.zero;
                    GameObject point = CreateNewFieldPoint(dummyPosition);
                    point.SetActive(false);
                    fieldPointPool.Add(point);

                }
            }

            // Create field points
            int poolIndex = 0;
            avgPos = Vector3.zero;
            count = 0;
            for (int i = 0; i < pointCountPerWave; i++) // Z-axis
            {
                for (int j = 0; j < waveRows; j++) // Y-axis
                {
                    for (int k = 0; k < waveRows; k++) // X-axis
                    {
                        Vector3 position = new Vector3(k * spaceBetweenWaves, j * spaceBetweenWaves, i * spacing);
                        avgPos += position;
                        count++;

                        GameObject point;
                        if (useObjectPooling)
                        {
                            // Get a point from the pool
                            point = fieldPointPool[poolIndex];
                            point.transform.localPosition = position;
                            point.SetActive(true);

                            // Get the corresponding arrows
                            electricFieldArrows.Add(electricArrowPool[poolIndex]);
                            magneticFieldArrows.Add(magneticArrowPool[poolIndex]);
                        }
                        else
                        {
                            // The original way to create points
                            point = new GameObject($"FieldPoint_{i}{j}{k}");
                            point.transform.parent = waveContainer.transform;
                            point.transform.localPosition = position;

                            electricFieldArrows.Add(CreateArrow("Electric", point));
                            magneticFieldArrows.Add(CreateArrow("Magnetic", point));
                        }

                        fieldPoints.Add(point);
                        poolIndex++;
                    }
                }
            }

            avgPos  /= count;


            if (debugMode)
                Debug.Log($"Created/activated {fieldPoints.Count} field points");
        }
        catch (System.Exception e)
        {
            Debug.LogError("Error creating field points: " + e.Message);
        }
    }
    // Helper function: create Arrow instances
    Arrow CreateArrow(string type, GameObject parent = null)
    {
        GameObject arrowObj = new GameObject(type + " Arrow");
        arrowObj.transform.parent = parent ? parent.transform : null;
        arrowObj.transform.localPosition = Vector3.zero;


        // Add arrow setup
        Arrow arrow = new Arrow(
            parent: arrowObj,
            initialDirection: type == "Electric" ? Vector3.up : Vector3.right,
            lengthOfTail: 0,
            relativeSize: arrowRelativeSize,
            headMaterial: type == "Electric" ? electricFieldMaterial : magneticFieldMaterial,
            tailMaterial: type == "Electric" ? electricFieldMaterial : magneticFieldMaterial
        );

        return arrow;
    }


    // Then update your UpdateFieldVectors method to check for peaks and change colors
    void UpdateFieldVectors()
    {
        try
        {
            // Update all field points
            for (int i = 0; i < fieldPoints.Count; i++)
            {
                // Skip inactive field points
                if (!fieldPoints[i].activeSelf)
                    continue;

                // get the position along the propagation axis (z-axis)
                float z = fieldPoints[i].transform.localPosition.z;
                float pointPhase = phase + (z / wavelength) * 2 * Mathf.PI;

                float eFieldStrength = amplitude * Mathf.Sin(pointPhase);
                float bFieldStrength = amplitude * Mathf.Sin(pointPhase);

                // Rest of your existing field vector update code...
                Transform eFieldTransform = fieldPoints[i].transform.GetChild(0);
                Transform bFieldTransform = fieldPoints[i].transform.GetChild(1);

                // Update electric field arrow (y-axis)
                eFieldTransform.rotation = Quaternion.identity;
                electricFieldArrows[i].SetTailLength(Mathf.Abs(eFieldStrength));

                // Check if peak coloring is enabled
                if (enablePeakColorChange)
                {
                    float normalizedEStrength = Mathf.Abs(eFieldStrength) / amplitude;
                    if (normalizedEStrength >= peakThreshold)
                    {
                        electricFieldArrows[i].SetHeadColor(peakColor);
                    }
                    else
                    {
                        electricFieldArrows[i].SetHeadColor(electricFieldMaterial.color);
                    }
                }

                // Set correct direction based on field sign
                if (eFieldStrength < 0)
                {
                    eFieldTransform.rotation = Quaternion.Euler(0, 0, 180);
                }
                electricFieldArrows[i].Update();

                // Update magnetic field arrow (x-axis)
                bFieldTransform.rotation = Quaternion.Euler(0, 0, 90);
                magneticFieldArrows[i].SetTailLength(Mathf.Abs(bFieldStrength));

                // Check if peak coloring is enabled
                if (enablePeakColorChange)
                {
                    float normalizedBStrength = Mathf.Abs(bFieldStrength) / amplitude;
                    if (normalizedBStrength >= peakThreshold)
                    {
                        magneticFieldArrows[i].SetHeadColor(peakColor);
                    }
                    else
                    {
                        magneticFieldArrows[i].SetHeadColor(magneticFieldMaterial.color);
                    }
                }

                // Set correct direction based on field sign
                if (bFieldStrength < 0)
                {
                    bFieldTransform.rotation = Quaternion.Euler(0, 0, 270);
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
        // Prevent division by zero
        if (newFrequency <= 0.001f)
        {
            newFrequency = 0.001f;
        }

        frequency = Mathf.Clamp(newFrequency, minFrequency, maxFrequency);

        // If keeping wave speed constant, adjust wavelength accordingly
        if (keepWaveSpeedConstant && frequency > 0)
        {
            float newWavelength = constantWaveSpeed / (2 * Mathf.PI * frequency);

            // Only update wavelength value
            wavelength = newWavelength;

            // Update wavelength UI elements
            if (wavelengthSlider != null)
            {
                // Make sure the value is within slider limits
                wavelengthSlider.value = Mathf.Clamp(newWavelength, minWavelength, maxWavelength);
            }

            if (wavelengthInput != null)
            {
                wavelengthInput.text = wavelength.ToString("0.000");
            }
        }

        // Update the frequency UI elements
        if (frequencySlider != null)
        {
            frequencySlider.value = frequency;
        }

        if (frequencyInput != null)
        {
            frequencyInput.text = frequency.ToString("0.000");
        }

        if (debugMode)
            Debug.Log($"Frequency set to {frequency}, Wavelength adjusted to {wavelength}, Wave speed: {2 * Mathf.PI * frequency * wavelength}");
    }
    public void SetAmplitude(float newAmplitude)
    {
        amplitude = Mathf.Clamp(newAmplitude, minAmplitude, maxAmplitude);

        if (amplitudeSlider != null && Mathf.Abs(amplitudeSlider.value - amplitude) > 0.01f)
        {
            amplitudeSlider.value = amplitude;
            amplitudeInput.text = amplitude.ToString();

            if (debugMode)
                Debug.Log($"Amplitude set to {amplitude}");
        }


    }

    public void SetWavelength(float newWavelength)
    {
        // Prevent division by zero
        if (newWavelength <= 0.001f)
        {
            newWavelength = 0.001f;
        }

        wavelength = Mathf.Clamp(newWavelength, minWavelength, maxWavelength);

        // If keeping wave speed constant, adjust frequency accordingly
        if (keepWaveSpeedConstant && wavelength > 0)
        {
            float newFrequency = constantWaveSpeed / (2 * Mathf.PI * wavelength);

            // Only update frequency value
            frequency = newFrequency;

            // Update frequency UI elements
            if (frequencySlider != null)
            {
                // Make sure the value is within slider limits
                frequencySlider.value = Mathf.Clamp(newFrequency, minFrequency, maxFrequency);
            }

            if (frequencyInput != null)
            {
                frequencyInput.text = frequency.ToString("0.000");
            }
        }

        // Update the wavelength UI elements
        if (wavelengthSlider != null)
        {
            wavelengthSlider.value = wavelength;
        }

        if (wavelengthInput != null)
        {
            wavelengthInput.text = wavelength.ToString("0.000");
        }

        if (debugMode)
            Debug.Log($"Wavelength set to {wavelength}, Frequency adjusted to {frequency}, Wave speed: {2 * Mathf.PI * frequency * wavelength}");
    }
    public void onFrequencyInputChanged(string value)
    {
        if (float.TryParse(value, out float result))
        {
            SetFrequency(result);
        }
    }

    public void onAmplitudeInputChanged(string value)
    {
        if (float.TryParse(value, out float result))
        {
            SetAmplitude(result);
        }
    }

    public void onWavelengthInputChanged(string value)
    {

        if (float.TryParse(value, out float result))
        {
            SetWavelength(result);
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
        // Remove listeners to prevent memory leaks
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

        // clear pools
        fieldPointPool.Clear();
        electricArrowPool.Clear();
        magneticArrowPool.Clear();
    }
}