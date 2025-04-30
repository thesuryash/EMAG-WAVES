using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LoadScene : MonoBehaviour
{
    [SerializeField] private List<ButtonScenePair> buttonScenePairs; // List of buttons and scene names

    private void Start()
    {
        foreach (var pair in buttonScenePairs)
        {
            if (pair.button != null && !string.IsNullOrEmpty(pair.sceneName))
            {
                string sceneName = pair.sceneName; // Capture the scene name for closure
                pair.button.onClick.AddListener(() => LoadScene_(sceneName));
            }
            else
            {
                Debug.LogError("Button or scene name is missing or invalid in one of the pairs.");
            }
        }
    }

    private void LoadScene_(string sceneName)
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
        public string sceneName; // Name of the Scene to load
    }
}