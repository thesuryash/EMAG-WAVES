using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PausePlay_EM_Waves : MonoBehaviour
{
    [SerializeField] private Button pausePlayButton;
    [SerializeField] private TextMeshProUGUI buttonText;
    [SerializeField] private bool isPaused = false; // False by default



    void Start()
    {
        pausePlayButton.onClick.AddListener(OnPausePlayClicked);

        // Initialize isPlaying based on the current pause state
        isPaused = !GlobalVariables.IsPaused;

        // Optionally initialize fluxScript if needed (uncomment this if required)
        // fluxScript = target.GetComponent<Flux>();
    }

    private void OnPausePlayClicked()
    {
        //Debug.Log("PausePlay button clicked. isPlaying: " + isPaused);

        if (!isPaused) // Pausing the simulation
        {
            GlobalVariables.IsPaused = true;


            buttonText.text = "Play";

            //Debug.Log("Simulation paused.");
        }
        else // Resuming the simulation
        {
            GlobalVariables.IsPaused = false;


            buttonText.text = "Pause";

            //Debug.Log("Simulation resumed.");
        }

        // Toggle isPlaying state
        isPaused = !isPaused;
    }

    void Update()
    {
        // Optionally add meaningful logic based on isPlaying
        //Debug.Log("Current state: isPaused = " + isPaused);

        //if (!isPaused) // Pausing the simulation
        //{
        //    buttonText.text = "Play";

        //}
        //else
        //{
        //    buttonText.text = "Pause";
        //}
    }
}