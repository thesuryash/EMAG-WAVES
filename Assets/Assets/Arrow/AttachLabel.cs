using UnityEngine;
using UnityEngine.ProBuilder;


public class Attach : MonoBehaviour
{
    [SerializeField] private GameObject thisLabel;
    [SerializeField] private GameObject head;
    [SerializeField] private float factor;
    private Camera _camera;

    // Scaling settings
    public float scaleFactor = 0.1f; // Adjust this to change how much the size changes with distance
    public float minScale = 0.5f;    // Minimum scale for the label
    public float maxScale = 5.0f;    // Maximum scale for the label

    void Start()
    {
        factor = 1f;
        _camera = Camera.main;
        if (_camera == null)
        {
            Debug.LogError("Main Camera not found!");
        }
    }

    void Update()
    {
        if (thisLabel == null || head == null || _camera == null)
        {
            Debug.LogError("One or more references are not set!");
            return; // Exit the method to prevent further errors
        }

        UpdateLabelPosition();
        UpdateLabelScale();
    }

    void UpdateLabelPosition()
    {
        // Position the label in front of the head
        thisLabel.transform.position = head.transform.position + (head.transform.forward * factor); // Adjust this to position correctly in front of the arrow
        thisLabel.transform.rotation = Quaternion.LookRotation(thisLabel.transform.position - _camera.transform.position);
    }

    void UpdateLabelScale()
    {
        // Calculate distance from the camera to the label
        float distance = Vector3.Distance(_camera.transform.position, thisLabel.transform.position);

        // Calculate a scale based on the distance
        float scale = Mathf.Clamp(distance * scaleFactor, minScale, maxScale);

        // Apply the calculated scale
        thisLabel.transform.localScale = Vector3.one * scale;
    }
}
