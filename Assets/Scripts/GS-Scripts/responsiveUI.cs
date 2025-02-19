using UnityEngine;

[RequireComponent(typeof(RectTransform))]
public class ResponsiveUI : MonoBehaviour
{
    private RectTransform rectTransform;

    void Start()
    {
        rectTransform = GetComponent<RectTransform>();
        AdjustSizeAndPosition();
    }

    void AdjustSizeAndPosition()
    {
        // Anchor the panel to the right-middle of the screen
        rectTransform.anchorMin = new Vector2(1, 0.5f); // Right-middle
        rectTransform.anchorMax = new Vector2(1, 0.5f); // Right-middle
        rectTransform.pivot = new Vector2(1, 0.5f);      // Right-middle pivot

        // Get the screen dimensions
        float screenWidth = Screen.width;
        float screenHeight = Screen.height;

        // Calculate the panel's size
        float panelWidth = screenWidth * 0.15f; // 15% of screen width
        float panelHeight = screenHeight * 0.5f; // 50% of screen height
        rectTransform.sizeDelta = new Vector2(panelWidth, panelHeight);

        // Position the panel on the right side, centered vertically
        float xOffset = -panelWidth / 2; // Offset by half the width (keeps it on-screen)
        rectTransform.anchoredPosition = new Vector2(xOffset, 0);
    }

    void Update()
    {
        // If you need to adjust dynamically based on resolution changes
        AdjustSizeAndPosition();
    }
}