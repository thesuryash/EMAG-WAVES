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
    [SerializeField] public float frequency;

    [SerializeField] public float length = 1.0f;
    [SerializeField] public float width = 1.0f;
    [SerializeField] public float rotation = 1.0f;

    /*[SerializeField]*/
    private GameObject frame;

    [SerializeField] public TMP_InputField electricFieldInput;
    [SerializeField] public Slider MagneticFieldSlider;

    [SerializeField] public TMP_Text fluxText;
    [SerializeField] public TMP_Text thetaText;
    [SerializeField] public TMP_Text areaText;


    [SerializeField] public Slider lengthSlider;
    [SerializeField] public TMP_InputField lengthInput;
    [SerializeField] public Slider widthSlider;
    [SerializeField] public TMP_InputField widthInput;
    [SerializeField] public Slider rotationSlider;
    [SerializeField] public TMP_InputField rotationInput;


    [SerializeField] private float electricFieldMagnitude = 1f;


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

    private void Awake()
    {
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
        electricFieldMagnitude = MagneticFieldSlider.value;
    }

    // Start is called before the first frame update

    void Start()
    {
        float initialLength = Arrow.CalculateLengthByValue(area);
        //areaArrow = new Arrow(areaArrowAttachedTo, areaArrowHead, areaArrowTail, arrowheadDirection, initialLengthOfTail);
        areaArrow = new Arrow(areaArrowAttachedTo, arrowheadDirection, initialLengthOfTail);


        //Setting up the sliders
        frame = GetComponent<GameObject>();


        float EFMagnitude = MagneticFieldSlider.value;
        float arrowLength = 1f;

        fieldArrowParent.AddComponent<MeshFilter>();

        //fieldArrow = new Arrow(fieldArrowParent, fieldArrowHead, fieldArrowTail, new Vector3(0, 0, 1), arrowLength);
        fieldArrow = new Arrow(fieldArrowParent, new Vector3(0, 0, 1), arrowLength);



        //Listeners
        MagneticFieldSlider.onValueChanged.AddListener(OnElectricFieldSliderChanged);
        lengthSlider.onValueChanged.AddListener(OnLengthSliderChanged);
        widthSlider.onValueChanged.AddListener(OnWidthSliderChanged);
        rotationSlider.onValueChanged.AddListener(OnRotationSliderChanged);
        rotationSlider.onValueChanged.AddListener(SnapToClosestPoint);
        lengthInput.text = 1f.ToString();
        widthInput.text = 1f.ToString();
        electricFieldInput.onEndEdit.AddListener(OnElectricFieldChanged);
        lengthInput.onEndEdit.AddListener(OnLengthInputChanged);
        widthInput.onEndEdit.AddListener(OnWidthInputChanged);
        rotationInput.onEndEdit.AddListener(OnRotationInputChanged);
        //pausePlayButton.onClick.AddListener(OnPausePlayClicked);

        // For mobile input
        electricFieldInput.onSelect.AddListener(ShowMobileKeyboard);
        lengthInput.onSelect.AddListener(ShowMobileKeyboard);
        widthInput.onSelect.AddListener(ShowMobileKeyboard);
        rotationInput.onSelect.AddListener(ShowMobileKeyboard);


        lengthSlider.onValueChanged.AddListener(delegate { CalculateFlux(); });
        widthSlider.onValueChanged.AddListener(delegate { CalculateFlux(); });
        rotationSlider.onValueChanged.AddListener(delegate { CalculateFlux(); });


        areaText.text = "Area: " + (lengthSlider.value * widthSlider.value).ToString("F2") + " m^2";

        CalculateFlux();
        RotateObject();


        //Arrow
        RotateObject();

        //For the area Arrow



        Debug.Log("Start method called.");


        //if (areaArrowAttachedTo == null || areaArrowTail == null || areaArrowHead == null || arrowGameObject == null)
        //{
        //    Debug.LogError("One or more references are not set in   the Inspector.");
        //    return;

        //}
        //Ensure that the arrow is instantiated with the correct parameters
        areaArrow.SetScene();
        areaArrow.SetTailLength(CalculateTailLengthByArea(CalculateArea(), 10f));

        Debug.Log("Arrow instantiated and scene set.");



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

    }

    void OnElectricFieldSliderChanged(float value)
    {
        float newElectricField = value;
        float currentElectricField = electricFieldMagnitude;
        electricFieldMagnitude = newElectricField;
        electricFieldInput.text = newElectricField.ToString();
        CalculateFlux();

    }

    void OnLengthSliderChanged(float value)
    {
        float newLength = value * 0.05f; // Adjust as needed
        float currentWidth = transform.localScale.z;
        Vector3 newScale = new Vector3(newLength, transform.localScale.y, currentWidth);
        transform.localScale = newScale;
        lengthInput.text = value.ToString();
    }

    void OnWidthSliderChanged(float value)
    {
        float newWidth = value * 0.05f; // Adjust as needed
        float currentLength = transform.localScale.x;
        Vector3 newScale = new Vector3(currentLength, transform.localScale.y, newWidth);
        transform.localScale = newScale;
        widthInput.text = value.ToString();
    }

    void OnRotationSliderChanged(float value)
    {
        rotationInput.text = value.ToString();
    }

    void RotateObject()
    {
        /*transform.Rotate(Vector3.up, rotationSlider.value * Time.deltaTime); */
        transform.rotation = Quaternion.Euler(0, rotationSlider.value + 180, 90);
    }



    void OnElectricFieldChanged(string value)
    {
        if (float.TryParse(value, out float result))
        {
            electricFieldMagnitude = result;
            MagneticFieldSlider.value = result;
            CalculateFlux();
        }
    }

    void OnLengthInputChanged(string value)
    {
        if (float.TryParse(value, out float result))
        {
            lengthSlider.value = result;
            CalculateFlux();
        }
    }

    void OnWidthInputChanged(string value)
    {
        if (float.TryParse(value, out float result))
        {
            widthSlider.value = result;
            CalculateFlux();
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

            transform.rotation = Quaternion.Euler(0, newRotation, 0);
        }
        else
        {
            Debug.LogError("Invalid input for rotation.");
            rotationInput.text = rotationSlider.value.ToString(); // Reset to previous value or default
        }
        CalculateFlux();

    }

    public float CalculateArea()
    {
        float area = lengthSlider.value * widthSlider.value;

        return area;
    }


    public float FluxValue(float B, float A, float theta)
    {
        return B * A * Mathf.Cos(theta * (Mathf.PI / 180));
    }

    void CalculateFlux()
    {
        float area = CalculateArea();

        float theta = rotationSlider.value - 90;

        float flux = FluxValue(electricFieldMagnitude, area, theta);
        /*flux = Mathf.Abs(flux) ;*/
        //float thetaTextValue = 180 - Mathf.Abs(180 - theta);

        float thetaTextValue = Mathf.Abs(180 - Mathf.Abs(90 - theta));


        fluxText.text = "Flux: " + flux.ToString("F2");

        thetaText.text = "Angle with +X axis: " + thetaTextValue.ToString("F2") + " \u00B0";
        electricFieldInput.text = electricFieldMagnitude.ToString("F2");

        areaText.text = "Area: " + area.ToString("F2") + " m\u00B2";

    }

    public float EMFValue(float frequency, float B, float A, float t)
    {
        float angle = frequency * t;
        float E = frequency * B * A * Mathf.Sin(angle * Mathf.PI / 180);

        return E;
    }

    void CalculateEMF()
    {
        float emf = EMFValue(frequency, MagneticFieldSlider.value, (lengthSlider.value * widthSlider.value), (float)Time.deltaTime);
    }

    public float CurrentValue(float Resistance)
    {
        float emf = EMFValue(frequency, MagneticFieldSlider.value, (lengthSlider.value * widthSlider.value), (float)Time.deltaTime);

        return emf / Resistance;
    }

    void CalculateCurrent()
    {

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

        float closestValue = snapPoints[0];
        float smallestDifference = Mathf.Abs(snapPoints[0] - value);

        foreach (float snapPoint in snapPoints)
        {
            float difference = Mathf.Abs(snapPoint - value);
            if (difference < smallestDifference)
            {
                smallestDifference = difference;
                closestValue = snapPoint;
            }
        }

        if (smallestDifference <= threshold)
        {
            rotationSlider.value = closestValue;
        }
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



