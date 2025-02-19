//using System.Collections;
//using System.Collections.Generic;
//using UnityEngine;

//public class OverlapEvent : MonoBehaviour
//{
//    // Start is called before the first frame update
//    [SerializeField] public Material originalMat;
//    [SerializeField] public Material TransparentMat;
//    void Start()
//    {
        
//    }
//    public float originalTransparency;
//    private void OnTriggerEnter(Collider other)
//    {
//        if(other.gameObject == null)
//        {
//            Debug.Log("something");
//        }
//        else
//        {
//            GameObject charge = other.gameObject.GetComponent<GameObject>();
//            Renderer chargeRend = charge.GetComponent<Renderer>();
//            chargeRend.material = TransparentMat;
//        }
//    }

//    private void OnTriggerExit(Collider other)
//    {
//        GameObject charge = other.gameObject.GetComponent<GameObject>();
//        Renderer chargeRend = charge.GetComponent<Renderer>();
//        chargeRend.material = originalMat;
//    }
//    void ChangeTransparency(GameObject obj, float alpha)
//    {
//        Renderer renderer = obj.GetComponent<Renderer>();
//        if (renderer != null)
//        {
//            Color color = renderer.material.color;
//            color.a = alpha;
//            renderer.material.color = color;
//        }
//    }

 

//    // Update is called once per frame
//    void Update()
//    {
        
//    }
//}
