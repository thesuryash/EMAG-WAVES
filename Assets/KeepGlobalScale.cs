using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KeepGlobalScale : MonoBehaviour
{
    [SerializeField] private Vector3 targetScale = new Vector3(0.2f, 0.2f, 0.2f);

    void Update()
    {
        if (transform.parent != null)
        {
            // Get the parent's current scale
            Vector3 parentScale = transform.parent.lossyScale;

            // 2. USE THE VARIABLE HERE (Note the capital 'S')
            // Calculate the inverse to counteract the parent's stretch
            transform.localScale = new Vector3(
                targetScale.x / parentScale.x,
                targetScale.y / parentScale.y,
                targetScale.z / parentScale.z
            );
        }
    }
}
