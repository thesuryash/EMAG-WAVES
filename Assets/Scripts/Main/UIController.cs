using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI; // Make sure to include this namespace

public class UIController : MonoBehaviour
{
    [SerializeField] private List<GameObjectScenePair> panelScenePairs; // List to hold pairs of panels and scene references

    private Dictionary<GameObject, string> panelSceneDictionary;

    private void Start()
    {
        panelSceneDictionary = new Dictionary<GameObject, string>();

        foreach (var pair in panelScenePairs)
        {
            if (pair.panel != null && !string.IsNullOrEmpty(pair.sceneReference.SceneName))
            {
                panelSceneDictionary.Add(pair.panel, pair.sceneReference.SceneName);

                // Ensure the panel has a Button component and attach the onClick listener
                Button panelButton = pair.panel.GetComponent<Button>();
                if (panelButton == null)
                {
                    panelButton = pair.panel.AddComponent<Button>();
                }
                panelButton.onClick.AddListener(() => LoadScene(pair.panel));
            }
            else
            {
                Debug.LogError("Panel or scene reference is missing in one of the pairs.");
            }
        }
    }

    private void LoadScene(GameObject panel)
    {
        if (panelSceneDictionary.TryGetValue(panel, out string sceneName))
        {
            SceneManager.LoadScene(sceneName);
        }
        else
        {
            Debug.LogError("Panel not found in the dictionary.");
        }
    }
}
