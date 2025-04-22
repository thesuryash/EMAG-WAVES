using UnityEngine;
using System;

#if UNITY_EDITOR
using UnityEditor;
#endif

//[Serializable]
public class SceneReference
{
#if UNITY_EDITOR
    [SerializeField]
    private SceneAsset sceneAsset;
#endif

    public string ScenePath
    {
        get
        {
#if UNITY_EDITOR
            return sceneAsset != null ? UnityEditor.AssetDatabase.GetAssetPath(sceneAsset) : string.Empty;
#else
            return string.Empty; // Return default value in builds
#endif
        }
    }

    public string SceneName
    {
        get
        {
#if UNITY_EDITOR
            return System.IO.Path.GetFileNameWithoutExtension(ScenePath);
#else
            return string.Empty; // Return default value in builds
#endif
        }
    }
}