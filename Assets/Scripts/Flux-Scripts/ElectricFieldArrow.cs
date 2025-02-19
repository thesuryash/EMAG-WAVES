using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class ElectricFieldArrow : MonoBehaviour
{
    [SerializeField] private GameObject fieldArrowParent;
    [SerializeField] private GameObject fieldArrowHead;
    [SerializeField] private GameObject fieldArrowTail;
    [SerializeField] private Slider electricFieldSlider;

    private Arrow fieldArrow;



    // Start is called before the first frame update
    void Start()
    {
        float EFMagnitude = electricFieldSlider.value;
        float arrowLength = 1f;

        fieldArrow = new Arrow(fieldArrowParent, fieldArrowHead, fieldArrowTail, new Vector3(0, 0, 1), arrowLength);

    }

    // Update is called once per frame
    void Update()
    {
        //fieldArrow.SetTailLength(CalculateLengthByElectricField(electricFieldSlider.value));
    }
}



