using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class Testing : MonoBehaviour
{

    [SerializeField] private GameObject parent;
    private Arrow arrow;
    // Start is called before the first frame update
    void Start()
    {
        arrow = new Arrow(parent, new Vector3(0, 1, 0), 1f);
        arrow.SetScene();

        //Debug.Log(RootPath);
    }

    // Update is called once per frame
    void Update()
    {

    }

    //public static string RootPath
    //{
    //    get
    //    {
    //        var g = AssetDatabase.FindAssets($"t:Script {nameof(Testing)}");
    //        return AssetDatabase.GUIDToAssetPath(g[0]);
    //    }
    //}
}
