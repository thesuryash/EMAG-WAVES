using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;

public class EMWAVE : MonoBehaviour
{
    // Wave properties
    public int points = 100;
    public float wavelength = 10.000f;
    public float amplitude = 2.000f;
    public float frequency = 2.000f;

    private LineRenderer electricField;
    private LineRenderer magneticField;
    private float time = 0f;

    [SerializeField] private Slider wavelengthSlider;
    [SerializeField] private Slider frequencySlider;
    [SerializeField] private Slider amplitudeSlider;
    
    [SerializeField] private TMP_InputField wavelengthInput;
    [SerializeField] private TMP_InputField frequencyInput;
    [SerializeField] private TMP_InputField amplitudeInput;

    private void Awake()
    {
        Debug.Log("Initializing Sliders...");

        if (wavelengthSlider == null) Debug.LogError("Wavelength Slider is NULL!");
        if (amplitudeSlider == null) Debug.LogError("Amplitude Slider is NULL!");
        if (frequencySlider == null) Debug.LogError("Frequency Slider is NULL!");
        if (wavelengthInput == null) Debug.LogError("Wavelength InputField is NULL!");
        if (amplitudeInput == null) Debug.LogError("Amplitude InputField is NULL!");
        if (frequencyInput == null) Debug.LogError("Frequency InputField is NULL!");

        // Adjust slider ranges
        wavelengthSlider.minValue = 5f;
        amplitudeSlider.minValue = 0f;
        frequencySlider.minValue = 0f;

        wavelengthSlider.maxValue = 15f;
        amplitudeSlider.maxValue = 10f;
        frequencySlider.maxValue = 10f;

        // Set initial values
        wavelengthSlider.value = wavelength;
        amplitudeSlider.value = amplitude;
        frequencySlider.value = frequency;

        UpdateInputFieldValueFromSlider(wavelengthInput, wavelengthSlider);
        UpdateInputFieldValueFromSlider(frequencyInput, frequencySlider);
        UpdateInputFieldValueFromSlider(amplitudeInput, amplitudeSlider);
    }

    void Start()
    {
        Debug.Log("Creating LineRenderers...");

        electricField = CreateLineRenderer(Color.red, "ElectricField");
        magneticField = CreateLineRenderer(Color.blue, "MagneticField");

        if (electricField == null || magneticField == null)
        {
            Debug.LogError("LineRenderers failed to initialize!");
            return;
        }

        wavelengthSlider.onValueChanged.AddListener(OnWavelengthSliderChanged);
        frequencySlider.onValueChanged.AddListener(OnFrequencySliderChanged);
        amplitudeSlider.onValueChanged.AddListener(OnAmplitudeSliderChanged);
    }

    void Update()
    {
        time += Time.deltaTime * frequency;

        if (electricField == null || magneticField == null)
        {
            Debug.LogError("LineRenderer is NULL in Update()!");
            return;
        }

        DrawWave(electricField, Vector3.up, amplitude);
        DrawWave(magneticField, Vector3.right, amplitude / 2);
    }

    void UpdateSliderValueFromInputField(Slider slider, TMP_InputField input)
    {
        if (string.IsNullOrEmpty(input.text)) return;

        float value;
        try
        {
            value = float.Parse(input.text);
            Debug.Log($"Parsed value: {value}");
        }
        catch (FormatException ex)
        {
            Debug.LogError($"Parsing error: {ex.Message}");
            return;
        }

        slider.value = Mathf.Clamp(value, slider.minValue, slider.maxValue);
    }

    void UpdateInputFieldValueFromSlider(TMP_InputField input, Slider slider)
    {
        try
        {
            float value = Mathf.Round(slider.value * 1000) / 1000; // Rounding to 3 decimal places
            input.text = value.ToString();
        }
        catch (Exception ex)
        {
            Debug.LogError($"An error occurred: {ex.Message}");
        }
    }

    LineRenderer CreateLineRenderer(Color color, string name)
    {
        GameObject lineObj = new GameObject(name);
        lineObj.transform.parent = transform;
        lineObj.transform.localPosition = Vector3.zero;

        LineRenderer lr = lineObj.AddComponent<LineRenderer>();

        lr.positionCount = points;
        lr.startWidth = 0.15f;  // Increased width for better visibility
        lr.endWidth = 0.15f;

        // Assign a material to ensure visibility
        Material lineMaterial = new Material(Shader.Find("Unlit/Color"));
        lineMaterial.color = color;
        lr.material = lineMaterial;

        lr.startColor = color;
        lr.endColor = color;
        lr.useWorldSpace = true;  // Ensure it's in 3D world space

        Debug.Log($"Created LineRenderer: {name} with {points} points.");
        return lr;
    }

    private void OnWavelengthSliderChanged(float value)
    {
        UpdateInputFieldValueFromSlider(wavelengthInput, wavelengthSlider);
    }

    private void OnAmplitudeSliderChanged(float value)
    {
        UpdateInputFieldValueFromSlider(amplitudeInput, amplitudeSlider);
    }

    private void OnFrequencySliderChanged(float value)
    {
        UpdateInputFieldValueFromSlider(frequencyInput, frequencySlider);
    }

    void DrawWave(LineRenderer lr, Vector3 axis, float amp)
    {
        if (lr == null)
        {
            Debug.LogError("LineRenderer is NULL in DrawWave()!");
            return;
        }

        for (int i = 0; i < points; i++)
        {
            float z = i * (wavelength / points);
            float y = amp * Mathf.Sin((z * Mathf.PI / wavelength) - time);
            Vector3 pos = axis * y + Vector3.forward * z;
            lr.SetPosition(i, pos);
        }
    }

    // Placeholder for future logic to distribute points
    private List<Vector3> DistributePoints()
    {
        List<Vector3> pointsList = new List<Vector3>();

        // TODO: Implement point distribution logic
        Debug.Log("DistributePoints() needs to be implemented!");

        return pointsList;
    }
}
