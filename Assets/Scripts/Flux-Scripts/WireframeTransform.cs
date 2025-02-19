using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.ProBuilder;
using UnityEngine.UI;

public class WireframeTransform : MonoBehaviour
{
    [SerializeField] public float length = 1.0f;
    [SerializeField] public float width = 1.0f;
    [SerializeField] public float rotation = 1.0f;

/*    private float minLength = 1.0f;
    private float maxLength = 10.0f;
    private float minWidth = 1.0f;
    private float maxWidth = 1.0f;
    private float minRotation = 0f;
    private float maxRotation = 360f;
*/
    [SerializeField] private Slider lengthSlider;
    [SerializeField] private Slider widthSlider;
    [SerializeField] private Slider rotationSlider;
    [SerializeField] public TMP_InputField rotationInput;

    [SerializeField] private ProBuilderMesh pbMesh;


    // Start is called before the first frame update
    void Start()
    {
        
        pbMesh = GetComponent<ProBuilderMesh>();

        lengthSlider.minValue = 1.0f;
        lengthSlider.maxValue = 10f;
        widthSlider.minValue = 1.0f;
        widthSlider.maxValue = 10.0f;
        rotationSlider.minValue = 0f;
        rotationSlider.maxValue = 360f;

        lengthSlider.value = 1f;
        widthSlider.value = 1f;
        rotationSlider.value = 0f;

        //Listners
        lengthSlider.onValueChanged.AddListener(OnLengthSliderChanged);
        widthSlider.onValueChanged.AddListener(OnWidthSliderChanged);
        rotationSlider.onValueChanged.AddListener(OnRotationSliderChanged);
        /*rotationInput.onEndEdit.AddListener(OnRotationInputChanged);*/
    }

    // Update is called once per frame
    void Update()
    {
        RotateObject();
    }

    void OnLengthSliderChanged(float value)
    {
        UpdateScale();
    }
    
    void OnWidthSliderChanged(float value)
    {
        UpdateScale();
    }

    void OnRotationSliderChanged(float value)
    {

    }

/*    void On*/

    void UpdateScale()
    {
        Vector3 scale = new Vector3(lengthSlider.value, transform.localScale.y, widthSlider.value);
        transform.localScale = scale;

/*        pbMesh.ToMesh();
        pbMesh.Refresh();*/
    }

    void RotateObject()
    {
        /*transform.Rotate(Vector3.up, rotationSlider.value * Time.deltaTime); */
        transform.rotation = Quaternion.Euler(0,90+rotationSlider.value,90);  
    }
    
}
