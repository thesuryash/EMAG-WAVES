//using System.Collections;
//using System.Collections.Generic;
//using UnityEngine;
//using UnityEngine.ProBuilder;
//using TMPro;
//using UnityEngine.UI;
//using System.Globalization;
//using System.Text.RegularExpressions;


//public class AreaArrow : MonoBehaviour
//{
//    private Arrow  areaArrow;
//    [SerializeField] private GameObject attachedTo;
//    [SerializeField] private ProBuilderMesh tail;
//    [SerializeField] private ProBuilderMesh head;


//    [SerializeField] private Vector3 arrowheadDirection = new Vector3(0, 1, 0);


//    float area;


//    [SerializeField] public Slider lengthSlider;
//    [SerializeField] public Slider widthSlider;


//    [SerializeField] public TMP_InputField lengthInput;
//    [SerializeField] public TMP_InputField widthInput;



//    // Start is called before the first frame update
//    void Start()
//    {
//        string lengthText = lengthInput.text;
//        string widthText = widthInput.text;
//        float length = 0;
//        float width = 0;


//        if (lengthText == null || widthText == null)
//        {
//            length = 1f; width = 1f;
//        }
//        else
//        {
//            length = float.Parse(Regex.Replace(lengthText, @"\p{C}+", string.Empty), CultureInfo.InvariantCulture);
//            width = float.Parse(Regex.Replace(widthText, @"\p{C}+", string.Empty), CultureInfo.InvariantCulture);
//        }


//        //area = Flux.CalculateAreaStatic();
//        area = length * width;


//        float initialLength = Arrow.CalculateLengthByArea(area);


//        if (attachedTo == null || tail == null || head == null)
//        {
//            Debug.LogError("One or more references are not set in the Inspector.");
//            return;
//        }


//        // Ensure that the arrow is instantiated with the correct parameters
//        areaArrow = new Arrow(attachedTo, head, tail, arrowheadDirection, initialLength);
//        if (areaArrow == null)
//        {
//            Debug.LogError("Failed to instantiate the arrow.");
//            return;
//        }


//        areaArrow.SetScene();
//        areaArrow.SetTailLength(Flux.CalculateTailLengthByArea(area, 10f));


//        Debug.Log("Arrow instantiated and scene set.");
//    }


//    // Update is called once per frame
//    void Update()
//    {
//        if (areaArrow == null)
//        {
//            Debug.LogError("Arrow is not instantiated. NULL NULL NULL --- Update()");
//            return;
//        }


//        areaArrow.SetTailLength(Flux.CalculateTailLengthByArea(area, 20f));


//        Debug.Log("area: " + area);


//        // Check if the parent transform has changed
//        if (areaArrow.IsParentTransformChanged())
//        {
//            // Destroy the attachedTo object if the parent transform has changed
//            /* Destroy(attachedTo);
//            Debug.Log("Parent object destroyed due to transform change."); */
//            areaArrow.UpdateParentTransform();
//        }


//        // Update the arrow position and state
//        areaArrow.Update();
//    }
//}





