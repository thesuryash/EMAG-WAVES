using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.ProBuilder;
using UnityEngine.UI;

public class Faraday : MonoBehaviour
{

    [Header("Faraday Values")]
    [SerializeField] public float Resistance;
    [SerializeField] public static float frequency;
    [SerializeField] public Slider frequencySlider;
    [SerializeField] public TMP_InputField frequencyInput;


    [SerializeField] public float length = 1.0f;
    [SerializeField] public float width = 1.0f;
    [SerializeField] public float rotation = 1.0f;

    /*[SerializeField]*/
    private GameObject frame;

    [SerializeField] public TMP_InputField electricFieldInput;
    [SerializeField] public Slider MagneticFieldSlider;

    [SerializeField] public TMP_Text fluxText;
    [SerializeField] public TMP_Text emfText;
    [SerializeField] public TMP_Text currentText;


    [SerializeField] public TMP_Text thetaText;
    [SerializeField] public TMP_Text areaText;

    [SerializeField] public GameObject cube;
    


    [SerializeField] public Slider lengthSlider;
    [SerializeField] public TMP_InputField lengthInput;
    [SerializeField] public Slider widthSlider;
    [SerializeField] public TMP_InputField widthInput;
    [SerializeField] public Slider rotationSlider;
    [SerializeField] public TMP_InputField rotationInput;


    [SerializeField] private float magneticFieldMagnitude = 1f;


    //Area Arrow
    private float area;
    [SerializeField] private GameObject areaArrowAttachedTo;
    //[SerializeField] private GameObject areaArrowTail;
    //[SerializeField] private GameObject areaArrowHead;
    [SerializeField] private Vector3 arrowheadDirection = new Vector3(0, 1, 0);
    [SerializeField] private float initialLengthOfTail = 1f;    // Length of the arrow
    [SerializeField] private GameObject arrowGameObject;        // The object where the script is attached

    private Arrow areaArrow;
    private Arrow fieldArrow;
    [SerializeField] private GameObject fieldArrowParent;

    //[SerializeField] private GameObject fieldArrowHead;
    //[SerializeField] private GameObject fieldArrowTail;

    //[SerializeField] private Button pausePlayButton;
    //[SerializeField] private bool isPlaying = false;

    [Header("Visualization Connection")]
    [SerializeField] private CurrentVisualizer currentVisualizer; // <--- Add [SerializeField]

    private float previousFlux = 0f;

    private void Awake()
    {
        frequency = 1f;
        Resistance = 10f;
        MagneticFieldSlider.minValue = 0f;
        MagneticFieldSlider.maxValue = 10f;
        lengthSlider.minValue = 1.0f;
        lengthSlider.maxValue = 2.0f;
        widthSlider.minValue = 1.0f;
        widthSlider.maxValue = 2.0f;
        rotationSlider.minValue = 0f;
        rotationSlider.maxValue = 360f;


        MagneticFieldSlider.value = 5f;
        lengthSlider.value = 1f;
        widthSlider.value = 1f;
        rotationSlider.value = 90f;
        rotationInput.text = "90.00";

        area = lengthSlider.value * widthSlider.value;
        magneticFieldMagnitude = MagneticFieldSlider.value;
    }

    // Start is called before the first frame update

    void Start()
    {
        float initialLength = Arrow.CalculateLengthByValue(area);
        areaArrow = new Arrow(areaArrowAttachedTo, arrowheadDirection, initialLengthOfTail);

        // Setting up the sliders
        frame = GetComponent<GameObject>(); // Note: GetComponent<GameObject> is redundant (gameObject is implicit), but harmless.

        float EFMagnitude = MagneticFieldSlider.value;
        float arrowLength = 1f;

        fieldArrowParent.AddComponent<MeshFilter>();
        fieldArrow = new Arrow(fieldArrowParent, new Vector3(0, 0, 1), arrowLength);

        // --- LISTENERS SETUP ---

        // 1. Basic Value Updates (Keep these)
        MagneticFieldSlider.onValueChanged.AddListener(OnElectricFieldSliderChanged);
        lengthSlider.onValueChanged.AddListener(OnLengthSliderChanged);
        widthSlider.onValueChanged.AddListener(OnWidthSliderChanged);
        rotationSlider.onValueChanged.AddListener(OnRotationSliderChanged);
        rotationSlider.onValueChanged.AddListener(SnapToClosestPoint);

        frequencySlider.onValueChanged.AddListener(OnFrequencySliderChanged);

        // Input Fields
        lengthInput.text = "1";
        widthInput.text = "1";
        electricFieldInput.onEndEdit.AddListener(OnElectricFieldChanged);
        lengthInput.onEndEdit.AddListener(OnLengthInputChanged);
        widthInput.onEndEdit.AddListener(OnWidthInputChanged);
        rotationInput.onEndEdit.AddListener(OnRotationInputChanged);
        frequencyInput.onEndEdit.AddListener(OnFrequencyInputChanged);

        // 2. CRITICAL CHANGE HERE:
        // Only call CalculateFlux() in the listeners.
        // Do NOT call CalculateEMF() or CalculateCurrent() here.
        // The Update() loop will detect the flux change automatically.

        lengthSlider.onValueChanged.AddListener(delegate { CalculateFlux(); });
        widthSlider.onValueChanged.AddListener(delegate { CalculateFlux(); });
        rotationSlider.onValueChanged.AddListener(delegate { CalculateFlux(); });
        MagneticFieldSlider.onValueChanged.AddListener(delegate { CalculateFlux(); });

        // Mobile inputs
        electricFieldInput.onSelect.AddListener(ShowMobileKeyboard);
        lengthInput.onSelect.AddListener(ShowMobileKeyboard);
        widthInput.onSelect.AddListener(ShowMobileKeyboard);
        rotationInput.onSelect.AddListener(ShowMobileKeyboard);

        // Initial Calculations
        areaText.text = "Area: " + (lengthSlider.value * widthSlider.value).ToString("F2") + " m^2";

        CalculateFlux();
        previousFlux = FaradayLawVariables.Flux; // Initialize previous flux

        CalculateEMF();
        CalculateCurrent();
        RotateObject();

        areaArrow.SetScene();
        areaArrow.SetTailLength(CalculateTailLengthByArea(CalculateArea(), 10f));

        Debug.Log("Arrow instantiated and scene set.");

        frequencyInput.text = System.Math.Round(frequencySlider.value, 3).ToString("F2");

    }

    private void OnFrequencySliderChanged(float arg0)
    {
        frequency = arg0;
    }

    private void OnFrequencyInputChanged(string arg0)
    {
        if (float.TryParse(arg0, out float result))
        {
            frequencySlider.value = result;
            frequency = float.Parse(arg0);
            CalculateFlux();
            CalculateEMF(); CalculateCurrent();
        }
    }

    private void OnPausePlayClicked()
    {
        //Debug.Log(isPlaying + "***********************************");
        //if (!isPlaying)
        //{
        //    isPlaying = true;
        //    float speed = 5f;
        //    RotatingWireframe(speed);
        //}
        //else
        //{
        //    isPlaying = false;
        //    RotatingWireframe(0f);
        //}
    }

    private void RotatingWireframe(float speed)
    {
        //    Quaternion initialRotation = transform.rotation;
        //    float rotationSpeed = speed * Mathf.PI / 10f;
        //    float elapsedTime = Time.deltaTime;
        //    Quaternion targetRotation = Quaternion.AngleAxis(rotationSpeed * elapsedTime, Vector3.up);

        //    // Smoothly interpolate towards the target rotation
        //    transform.rotation = Quaternion.RotateTowards(transform.rotation, initialRotation * targetRotation, rotationSpeed * Time.deltaTime);
    }
    // Update is called once per frame
    void Update()

    {
        areaArrow.SetTailLength(CalculateTailLengthByArea(CalculateArea(), 20f));
        RotateObject();

        fieldArrow.SetTailLength(CalculateLengthByElectricField(MagneticFieldSlider.value, 20f));


        //Debug.Log("area: " + area);
        if (areaArrow == null)
        {
            Debug.LogError("Arrow1 is not instantiated. NULL NULL NULL --- Update()");
            return;
        }

        // Check if the parent transform has changed
        if (areaArrow.IsParentTransformChanged())
        {

            areaArrow.UpdateParentTransform();
        }



        // Update the arrow position and state
        areaArrow.Update();
        fieldArrow.Update();


        //pausePlayButton.onClick.AddListener(OnPausePlayClicked);

        CalculateFlux();
        CalculateEMF();
        CalculateCurrent();

       

    }

    void OnElectricFieldSliderChanged(float value)
    {
        float newElectricField = value;
        float currentElectricField = magneticFieldMagnitude;
        magneticFieldMagnitude = newElectricField;
        electricFieldInput.text = newElectricField.ToString();
        CalculateFlux();
        CalculateEMF(); CalculateCurrent();

    }

    void OnLengthSliderChanged(float value)
    {
        float newLength = value * 0.05f; // Adjust as needed
        float currentWidth = cube.transform.localScale.z;
        Vector3 newScale = new Vector3(newLength, cube.transform.localScale.y, currentWidth);
        cube.transform.localScale = newScale;
        lengthInput.text = value.ToString();
    }

    void OnWidthSliderChanged(float value)
    {
        float newWidth = value * 0.05f; // Adjust as needed
        float currentLength = cube.transform.localScale.x;
        Vector3 newScale = new Vector3(currentLength, cube.transform.localScale.y, newWidth);
        cube.transform.localScale = newScale;
        widthInput.text = value.ToString();
    }

    void OnRotationSliderChanged(float value)
    {
        rotationInput.text = value.ToString();
    }

    void RotateObject()
    {
        /*transform.Rotate(Vector3.up, rotationSlider.value * Time.deltaTime); */
        cube.transform.rotation = Quaternion.Euler(0, rotationSlider.value + 180, 90);
    }



    void OnElectricFieldChanged(string value)
    {
        if (float.TryParse(value, out float result))
        {
            magneticFieldMagnitude = result;
            MagneticFieldSlider.value = result;
            CalculateFlux();
            CalculateEMF(); CalculateCurrent();
        }
    }

    void OnLengthInputChanged(string value)
    {
        if (float.TryParse(value, out float result))
        {
            lengthSlider.value = result;
            CalculateFlux();
            CalculateEMF(); CalculateCurrent();
        }
    }

    void OnWidthInputChanged(string value)
    {
        if (float.TryParse(value, out float result))
        {
            widthSlider.value = result;
            CalculateFlux();
            CalculateEMF(); CalculateCurrent();
        }
    }

    void OnRotationInputChanged(string value)
    {
        if (float.TryParse(value, out float newRotation))
        {
            // Clamp the rotation to be between 0 and 360 degrees
            newRotation = Mathf.Clamp(newRotation % 360, 0, 360);
            rotationSlider.value = newRotation; // Assuming you also want to update a slider
            rotationInput.text = newRotation.ToString(); // Update the input field with clamped value

            cube.transform.rotation = Quaternion.Euler(0, newRotation, 0);
        }
        else
        {
            Debug.LogError("Invalid input for rotation.");
            rotationInput.text = rotationSlider.value.ToString(); // Reset to previous value or default
        }
        CalculateFlux();
        CalculateEMF(); CalculateCurrent();

    }

    public float CalculateArea()
    {
        float area = lengthSlider.value * widthSlider.value;

        return area;
    }


    public float FluxValue(float B, float A, float theta)
    {
        return B * A * Mathf.Sin(theta * (Mathf.PI / 180));
    }

    void CalculateFlux()
    {
        float area = CalculateArea();

        float theta = rotationSlider.value - 90;

        float flux = FluxValue(magneticFieldMagnitude, area, theta);
        /*flux = Mathf.Abs(flux) ;*/
        //float thetaTextValue = 180 - Mathf.Abs(180 - theta);

        float thetaTextValue = Mathf.Abs(180 - Mathf.Abs(90 - theta));

        FaradayLawVariables.Flux = flux;

        fluxText.text = "Flux: " + flux.ToString("F2");

        thetaText.text = "Angle with +X axis: " + thetaTextValue.ToString("F2") + " \u00B0";
        electricFieldInput.text = magneticFieldMagnitude.ToString("F2");

        areaText.text = "Area: " + area.ToString("F2") + " m\u00B2";

    }

    // Faraday.cs

    // CHANGE: Now takes 'currentAngle' instead of time
    public float EMFValue(float frequency, float B, float A, float currentAngle)
    {
        // The "Frequency" here controls the Amplitude (Peak Voltage)
        // The "Sin" depends on where the cube actually is (currentAngle)

        // Convert degrees to radians for the math
        float E = (float)System.Math.Round(frequency * B * A * Mathf.Sin(currentAngle * Mathf.Deg2Rad), 3);
        return E;
    }

    // 1. Update CalculateEMF
    // Faraday.cs

    void CalculateEMF()
    {
        // 1. Get current Flux
        float currentFlux = FaradayLawVariables.Flux;

        // 2. Calculate Induction (Change in Flux / Time)
        // This naturally becomes 0 if the cube stops rotating
        float deltaFlux = currentFlux - previousFlux;
        float emf = -deltaFlux / Time.deltaTime;

        // 3. Store for next frame
        previousFlux = currentFlux;

        // 4. Update UI
        FaradayLawVariables.EMF = emf;
        if (emfText) emfText.text = $"EMF: " + emf.ToString("F3");
    }

    public float CurrentValue(float Resistance)
    {
        // We simply use the EMF we just calculated above
        return FaradayLawVariables.EMF / Resistance;
    }

    void CalculateCurrent()
    {
        float current = CurrentValue(Resistance);
        FaradayLawVariables.Current = current;
        // use this wherever suitable
        currentText.text = $"Current: " + current.ToString();


        if (currentVisualizer != null)
        {
            // This automatically sets speed and direction (negative = reverse)
            currentVisualizer.currentAmps = current;
        }
    }

    public float CalculateTailLengthByArea(float area, float maxLength)
    {
        if (area > 0 && maxLength > 0)
        {
            // Define the range of the area that influences the tail length
            float maxArea = 50f; // Maximum considered area
            float minArea = 1f;  // Minimum considered area - avoids too small length

            // Ensure area is within the bounds
            float clampedArea = Mathf.Clamp(area, minArea, maxArea);

            // Calculate proportionate length
            float length = (maxLength / maxArea) * clampedArea;

            // Further clamp the length to make sure it is not longer than maxLength
            // and not shorter than a reasonable minimum (e.g., maxLength * 0.1 or some other factor)
            float minLength = maxLength * 0.1f; // Minimum length is 10% of maxLength
            return Mathf.Clamp(length, minLength, maxLength);
        }
        return 0; // If invalid area or maxLength, return 0
    }

    public float[] snapPoints;
    public float threshold = 5f; // Sensitivity of snapping


    private void SnapToClosestPoint(float value)
    {

        //float closestValue = snapPoints[0];
        //float smallestDifference = Mathf.Abs(snapPoints[0] - value);

        //foreach (float snapPoint in snapPoints)
        //{
        //    float difference = Mathf.Abs(snapPoint - value);
        //    if (difference < smallestDifference)
        //    {
        //        smallestDifference = difference;
        //        closestValue = snapPoint;
        //    }
        //}

        //if (smallestDifference <= threshold)
        //{
        //    rotationSlider.value = closestValue;
        //}
    }

    private void ShowMobileKeyboard(string text)
    {
        TouchScreenKeyboard.Open("", TouchScreenKeyboardType.Default);
    }


    public float CalculateLengthByElectricField(float field, float maxLength)
    {
        if (field >= 0 && maxLength > 0)
        {
            // Define the range of the area that influences the tail length
            float maxField = 100f; // Maximum considered area
            float minField = 0f;  // Minimum considered area - avoids too small length

            // Ensure area is within the bounds
            float clampedField = Mathf.Clamp(field, minField, maxField);

            // Calculate proportionate length
            float length = (maxLength / maxField) * clampedField;

            // Further clamp the length to make sure it is not longer than maxLength
            // and not shorter than a reasonable minimum (e.g., maxLength * 0.1 or some other factor)
            float minLength = maxLength * 0.1f; // Minimum length is 10% of maxLength

            return Mathf.Clamp(length, minLength, maxLength);
        }
        return 0; // If invalid area or maxLength, return 0
    }

}



