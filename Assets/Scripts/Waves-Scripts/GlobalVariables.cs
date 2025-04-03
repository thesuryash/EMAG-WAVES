using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GlobalVariables : MonoBehaviour
{
    public static float lambda = 5f;
    public static float omega = 5f;
    public static float E0 = 10.0f;
    public static float B0 = 10.0f;

    public static float k => 2 * Mathf.PI / lambda;

    private static bool _isPaused = false; // Backing field for 'IsPaused'
    private static float _deltaTime = 0f; // Backing field for 'DeltaTime'

    // Property for 'IsPaused'
    public static bool IsPaused
    {
        get => _isPaused;
        set
        {
            _isPaused = value;
            _deltaTime = _isPaused ? 0 : Time.deltaTime; // Adjust deltaTime when pausing/resuming
        }
    }

    // Property for 'DeltaTime'
    public static float DeltaTime
    {
        get => _deltaTime;
        set => _deltaTime = Mathf.Max(0, value); // Prevent negative deltaTime
    }

    private void Update()
    {
        // Dynamically update deltaTime if the game is not paused
        if (!_isPaused)
        {
            _deltaTime = Time.deltaTime;
        }
    }
}