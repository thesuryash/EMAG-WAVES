using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class UIController : MonoBehaviour
{
    [SerializeField] private List<ButtonScenePair> buttonScenePairs; // List of button-scene pairs

    private void Start()
    {
        foreach (var pair in buttonScenePairs)
        {
            if (pair.button != null && !string.IsNullOrEmpty(pair.sceneName))
            {
                string sceneName = pair.sceneName; // Capture the scene name for closure
                pair.button.onClick.AddListener(() => LoadScene(sceneName));
            }
            else
            {
                Debug.LogError("Button or scene name is missing or invalid.");
            }
        }
    }

    private void LoadScene(string sceneName)
    {
        if (!string.IsNullOrEmpty(sceneName))
        {
            SceneManager.LoadScene(sceneName);
        }
        else
        {
            Debug.LogError("Scene name is invalid or empty.");
        }
    }

    [System.Serializable]
    public class ButtonScenePair
    {
        public Button button; // Reference to the Button
        public string sceneName; // Scene name to load
    }
}