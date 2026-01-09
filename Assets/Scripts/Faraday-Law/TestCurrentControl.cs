using UnityEngine;

public class TestCurrentControl : MonoBehaviour
{
    [Header("Test Controls")]
    public CurrentVisualizer visualizer;

    [Tooltip("Drag this left/right to simulate induced current.")]
    [Range(-50f, 50f)]
    public float simulatedCurrent = 0f;

    void Update()
    {
        if (visualizer != null)
        {
            // Inject our slider value directly into the visualizer
            visualizer.currentAmps = simulatedCurrent;
        }
    }
}   