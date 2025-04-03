using UnityEngine;
using System;
using UnityEditor;

//[Serializable]
public class SceneReference
{
    [SerializeField]
    private SceneAsset sceneAsset;

    public string ScenePath => sceneAsset != null ? UnityEditor.AssetDatabase.GetAssetPath(sceneAsset) : string.Empty;

    public string SceneName => System.IO.Path.GetFileNameWithoutExtension(ScenePath);
}
