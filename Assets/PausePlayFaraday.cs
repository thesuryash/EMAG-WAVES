using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PausePlayFaraday : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private Button pausePlayButton;
    [SerializeField] private TextMeshProUGUI buttonText;

    [Header("Control Settings")]
    [SerializeField] private bool isPlaying = false;
    [SerializeField] private GameObject target;
    [SerializeField] private float rotationSpeed = 50f;

    // References to the Faraday script and its UI
    private Faraday faradayScript;
    [SerializeField] private Slider frequencySlider;
    [SerializeField] private TMP_InputField frequencyInput;

    [SerializeField] private Slider rotationSlider;
    [SerializeField] private TMP_InputField rotationInput;

    void Start()
    {
        // 1. Get the Faraday component (replacing 'Flux')
        faradayScript = target.GetComponent<Faraday>();

        // 2. Setup Button Listener
        pausePlayButton.onClick.AddListener(OnPausePlayClicked);

        // 3. Setup Frequency Sliders
        frequencySlider.maxValue = 150f;
        frequencySlider.minValue = 0.0f;
        frequencySlider.value = 50f;

        // Sync initial speed
        rotationSpeed = frequencySlider.value;

        frequencySlider.onValueChanged.AddListener(OnFrequencySliderChanged);
        frequencyInput.onEndEdit.AddListener(OnFrequencyInputChanged);
    }

    private void OnPausePlayClicked()
    {
        Debug.Log("PausePlay button clicked. isPlaying: " + !isPlaying); // Log the NEW state

        if (!isPlaying)
        {
            // SWITCH TO PLAYING
            isPlaying = true;

            // Disable manual slider control while playing
            rotationSlider.interactable = false;
           rotationInput.interactable = false;
            Debug.Log("Rotation started.");
        }
        else
        {
            // SWITCH TO PAUSED
            isPlaying = false;

            // Re-enable manual slider control
            rotationSlider.interactable = true;
            rotationInput.interactable = true;
            Debug.Log("Rotation stopped.");

            // Ensure final rotation is applied
            UpdateRotation();
        }
    }

    private void OnFrequencySliderChanged(float val)
    {
        // Update local rotation speed
        rotationSpeed = val;

        // Update Physics in Faraday script
        Faraday.frequency = val;

        // Update UI Text
        if (frequencyInput) frequencyInput.text = val.ToString("F2");
    }

    private void OnFrequencyInputChanged(string val)
    {
        if (float.TryParse(val, out float result))
        {
            frequencySlider.value = result; // This triggers the slider listener above
        }
    }

    // This logic is now exactly the same as the original PausePlay script
    private void UpdateRotation()
    {
        // 1. Get current value
        float currentRotation = rotationSlider.value;

        // 2. Calculate new value (Simple one-line logic)
        float newRotation = (currentRotation + rotationSpeed * Time.deltaTime) % 360;

        // 3. Apply it back to the slider
        rotationSlider.value = newRotation;
    }

    void Update()
    {
        if (isPlaying)
        {
            UpdateRotation();
            buttonText.text = "Pause";
        }
        else
        {
            buttonText.text = "Play";
        }
    }
}