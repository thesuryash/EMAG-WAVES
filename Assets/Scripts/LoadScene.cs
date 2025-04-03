using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LoadScene : MonoBehaviour
{
    [SerializeField] private List<ButtonScenePair> buttonScenePairs; // List of buttons and scene references

    private void Start()
    {
        foreach (var pair in buttonScenePairs)
        {
            if (pair.button != null && pair.sceneReference != null && !string.IsNullOrEmpty(pair.sceneReference.SceneName))
            {
                // Attach the onClick listener to the button
                pair.button.onClick.AddListener(() => LoadScene_(pair.sceneReference.SceneName));
            }
            else
            {
                Debug.LogError("Button or scene reference is missing in one of the pairs.");
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
        public Button button; // Reference to the button
        public SceneReference sceneReference; // Reference to the scene
    }

    [System.Serializable]
    public class SceneReference
    {
        [SerializeField] private Object sceneAsset; // Reference to the SceneAsset

        public string SceneName
        {
            get
            {
#if UNITY_EDITOR
                // Ensure the object is a scene and retrieve its name
                if (sceneAsset != null)
                {
                    string path = UnityEditor.AssetDatabase.GetAssetPath(sceneAsset);
                    if (!string.IsNullOrEmpty(path) && path.EndsWith(".unity"))
                    {
                        return System.IO.Path.GetFileNameWithoutExtension(path);
                    }
                }
#endif
                return string.Empty;
            }
        }
    }
}