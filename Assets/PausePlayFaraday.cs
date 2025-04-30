using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PausePlayFaraday : MonoBehaviour
{
    [SerializeField] private Button pausePlayButton;
    [SerializeField] private TextMeshProUGUI buttonText;
    [SerializeField] private bool isPlaying = false;
    [SerializeField] private GameObject target;
    private float rotationSpeed = 50f;
    private Flux fluxScript;
    [SerializeField] private Slider frequencySlider;
    [SerializeField] private TMP_InputField frequencyInput;
    private float frequency;

    void Start()
    {
        pausePlayButton.onClick.AddListener(OnPausePlayClicked);
        fluxScript = target.GetComponent<Flux>();

        frequencySlider.maxValue = 150f;
        frequencySlider.minValue = 0.0f;

        frequencySlider.onValueChanged.AddListener(OnFrequencySliderChanged);
        frequencyInput.onEndEdit.AddListener(OnFrequencyInputChanged);

        frequencySlider.value = 50f;


    }

    private void OnPausePlayClicked()
    {
        Debug.Log("PausePlay button clicked. isPlaying: " + isPlaying);
        if (!isPlaying)
        {
            isPlaying = true;
            fluxScript.rotationSlider.interactable = false; // Disable slider
            fluxScript.rotationInput.interactable = false; // Disable input field
            Debug.Log("Rotation started.");
        }
        else
        {
            isPlaying = false;
            fluxScript.rotationSlider.interactable = true; // Enable slider
            fluxScript.rotationInput.interactable = true; // Enable input field
            Debug.Log("Rotation stopped.");
            UpdateRotation();
        }
    }

    private void OnFrequencySliderChanged(float arg0)
    {
        frequency = arg0;
        Faraday.frequency = arg0;
        rotationSpeed = frequency;
        frequencyInput.text = arg0.ToString();
    }

    private void OnFrequencyInputChanged(string arg0)
    {
        if (float.TryParse(arg0, out float result))
        {
            frequencySlider.value = result;
            frequency = float.Parse(arg0);

            Faraday.frequency = float.Parse(arg0);
            rotationSpeed = frequency;
        }
    }

    private void UpdateRotation()
    {
        float newRotation = (fluxScript.rotationSlider.value + rotationSpeed * Time.deltaTime) % 360;
        fluxScript.rotationSlider.value = newRotation; // This will trigger the listener in the Flux script
    }

    void Update()
    {
        if (isPlaying)
        {
            UpdateRotation();
        }

        if (isPlaying)
        {
            buttonText.text = "Pause";
        }
        else
        {
            buttonText.text = "Play";
        }
    }
}



