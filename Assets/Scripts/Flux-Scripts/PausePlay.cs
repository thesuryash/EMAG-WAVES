using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PausePlay : MonoBehaviour
{
    [SerializeField] private Button pausePlayButton;
    [SerializeField] private TextMeshProUGUI buttonText;
    [SerializeField] private bool isPlaying = false;
    [SerializeField] private GameObject target;
    private float rotationSpeed = 50f;
    private Flux fluxScript;

    void Start()
    {
        pausePlayButton.onClick.AddListener(OnPausePlayClicked);
        fluxScript = target.GetComponent<Flux>();
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



