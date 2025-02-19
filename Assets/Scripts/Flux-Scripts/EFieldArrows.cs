using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EFieldArrows : MonoBehaviour
{
    private Transform eFieldArrows;
    // Start is called before the first frame update
    void Start()
    {
        eFieldArrows = GetComponent<Transform>();
        List<Transform> children = new List<Transform>();
        foreach (Transform child in eFieldArrows)
        {
            children.Add(child);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
