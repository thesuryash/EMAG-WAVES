using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.ProBuilder.Shapes;
//using static UnityEditor.PlayerSettings;
using System.Drawing;
using UnityEngine.UI;
using Unity.VisualScripting;
using TMPro;
using System;

public class CurrentGSSize
{
    public float? _length;
    public float?_radius;
    public float? _height;
    public float? _width;
    public string _name;
    public CurrentGSSize(float? length=null, float? width = null, float? height=null, float? radius=null, string name=null)
    {
        this._length = length;
        this._radius = radius;
        this._height = height;
        this._width = width;
        this._name = name;
    }

    public float? getLength()
    {
        return this._length;
    }

    public string getName()
    {
        return this._name;
    }
    public float? getRadius() {
        return this._radius;
    }

    public float? getHeight()
    {
        return this._height;
    }

    public float? getWidth()
    {
        return this._width;
    }
}

public class CurrentChargeSize
{
    public float? _radius; 
    public float? _length; 
    public float? _width; 
    public float? _height; 
    public string _name; 

    // Constructor to initialize
    public CurrentChargeSize(float? radius = null, float? length = null, float? width = null, float? height = null, string name = "")
    {
        this._radius = radius;
        this._length = length;
        this._width = width;
        this._height = height;
        this._name = name;
    }

    // Getters
    public float? getRadius() { return this._radius; }
    public float? getLength() { return this._length; }
    public float? getWidth() { return this._width; }
    public float? getHeight() { return this._height; }
    public string getName() { return this._name; }
}



public class Control : MonoBehaviour
{

    [SerializeField] public GameObject pointCharge;
    [SerializeField] public GameObject lineCharge;
    [SerializeField] public GameObject planeCharge;
    [SerializeField] public GameObject sphereGS;
    [SerializeField] public GameObject cylinderGS;
    [SerializeField] public GameObject cubeGS;

    [SerializeField] public Toggle noChargeToggle;
    [SerializeField] public Toggle pointChargeToggle;
    [SerializeField] public Toggle lineChargeToggle;
    [SerializeField] public Toggle planeChargeToggle;

    [SerializeField] public Toggle noGSToggle;
    [SerializeField] public Toggle sphereGSToggle;
    [SerializeField] public Toggle cylinderGSToggle;
    [SerializeField] public Toggle cubeGSToggle;

    [SerializeField] public GameObject SphereDimensions;
    [SerializeField] public Slider sphereRadiusSlider;
    [SerializeField] public TMP_InputField sphereRadiusInput;

    [SerializeField] public GameObject CylinderDimensions;
    [SerializeField] public Slider cylinderRadiusSlider;
    [SerializeField] public Slider cylinderLengthSlider;
    [SerializeField] public TMP_InputField cylinderRadiusInput;
    [SerializeField] public TMP_InputField cylinderLengthInput;

    [SerializeField] public GameObject CubeDimensions;
    [SerializeField] public Slider cubeRadiusSlider;
    [SerializeField] public Slider cubeLengthSlider;
    [SerializeField] public TMP_InputField cubeRadiusInput;
    [SerializeField] public TMP_InputField cubeLengthInput;

    public (Slider, TMP_InputField)[] gsRadius;
    public (Slider, TMP_InputField)[] gsLength;

    [SerializeField] public Material defaultChargeMaterial;
    [SerializeField] public Material smallChargeMaterial;    


    public List<GameObject> focusCharge;

    float initScale;

    public float transparencyOriginalGS;
    public float transparencyOriginalCharge;


    public enum ChargeType
    {
        noCharge, point, linear, planar, unevenSrf
    }
    public enum GaussSrfType
    {
        noSrf, sphere, cylinder, cube
    }

    public ChargeType chargeT = ChargeType.point;
    public GaussSrfType GaussSrfT = GaussSrfType.noSrf;




    private void Awake()
    {
        gsRadius = new (Slider, TMP_InputField)[] { (sphereRadiusSlider, sphereRadiusInput), (cylinderRadiusSlider, cylinderRadiusInput), (cubeRadiusSlider, cubeRadiusInput) };
        gsLength = new (Slider, TMP_InputField)[] { (cylinderLengthSlider, cylinderLengthInput), (cubeLengthSlider, cubeLengthInput) };

        foreach (var (sizeSlider, sizeInput) in gsRadius)
        {
            sizeSlider.minValue = 0.1f;
            sizeSlider.maxValue = 3f;
            sizeSlider.value = 1.0f;
            sizeInput.text = sizeSlider.value.ToString();
        }

        foreach (var (sizeSlider, sizeInput) in gsLength)
        {
            sizeSlider.minValue = 0.1f;
            sizeSlider.maxValue = 3f;
            sizeSlider.value = 1.0f;
            sizeInput.text = sizeSlider.value.ToString();
        }
        initScale = sphereGS.transform.localScale.x;


    }


    void Start()
    {
        pointChargeToggle.isOn = true;
        sphereGSToggle.isOn = true;
        chargeT = ChargeType.point;
        GaussSrfT = GaussSrfType.sphere;

        UpdateActiveObject();

        transparencyOriginalGS = cubeGS.GetComponent<Renderer>().material.color.a;
        transparencyOriginalCharge = planeCharge.GetComponent<Renderer>().material.color.a;

        noChargeToggle.onValueChanged.AddListener(delegate { OnNoChargeToggleOn(); });
        pointChargeToggle.onValueChanged.AddListener(delegate { OnPointChargeToggleOn(); });
        lineChargeToggle.onValueChanged.AddListener(delegate { OnLineChargeToggleOn(); });
        planeChargeToggle.onValueChanged.AddListener(delegate { OnPlaneChargeToggleOn(); });

        noGSToggle.onValueChanged.AddListener(delegate { OnNoGSToggleOn(); });
        sphereGSToggle.onValueChanged.AddListener(delegate { OnSphereGSToggleOn(); });
        cylinderGSToggle.onValueChanged.AddListener(delegate { OnCylinderGSToggleOn(); });
        cubeGSToggle.onValueChanged.AddListener(delegate { OnCubeGSToggleOn(); });

        foreach (var (sizeSlider, sizeInput) in gsRadius)
        {
            sizeSlider.onValueChanged.AddListener(OnRadiusSliderChanged);
            sizeInput.onEndEdit.AddListener(OnRadiusInputChanged);
        }

        foreach (var (sizeSlider, sizeInput) in gsLength)
        {
            sizeSlider.onValueChanged.AddListener(OnLengthSliderChanged);
            sizeInput.onEndEdit.AddListener(OnLengthInputChanged);
        }
    }

    void Update()
    {
        
        //UpdateChargeSize();

    }

    void OnNoChargeToggleOn()
    {
        chargeT = ChargeType.noCharge; 
        UpdateActiveObject(); 
    }

    void OnPointChargeToggleOn()
    {
        chargeT = ChargeType.point;
        UpdateActiveObject();
    }

    void OnLineChargeToggleOn()
    {
        chargeT = ChargeType.linear;
        UpdateActiveObject();
    }

    void OnPlaneChargeToggleOn()
    {
        chargeT = ChargeType.planar;
        UpdateActiveObject();
    }

    void OnNoGSToggleOn()
    {
        GaussSrfT = GaussSrfType.noSrf;
        UpdateActiveObject();
    }

    void OnSphereGSToggleOn()
    {
        GaussSrfT = GaussSrfType.sphere;
        UpdateActiveObject();
    }

    void OnCylinderGSToggleOn()
    {
        GaussSrfT = GaussSrfType.cylinder;
        UpdateActiveObject();
    }

    void OnCubeGSToggleOn()
    {
        GaussSrfT = GaussSrfType.cube;
        UpdateActiveObject();
    }

   public CurrentGSSize currentGaussSurfaceSize;
    void UpdateActiveObject()
    {

        pointCharge.SetActive(chargeT == ChargeType.point);
        lineCharge.SetActive(chargeT == ChargeType.linear);
        planeCharge.SetActive(chargeT == ChargeType.planar);

        sphereGS.SetActive(GaussSrfT == GaussSrfType.sphere);
        cylinderGS.SetActive(GaussSrfT == GaussSrfType.cylinder);
        cubeGS.SetActive(GaussSrfT == GaussSrfType.cube);

        SphereDimensions.SetActive(GaussSrfT == GaussSrfType.sphere);
        CylinderDimensions.SetActive(GaussSrfT == GaussSrfType.cylinder);
        CubeDimensions.SetActive(GaussSrfT == GaussSrfType.cube);


        switch (GaussSrfT)
        {
            case GaussSrfType.sphere:
                currentGaussSurfaceSize = new CurrentGSSize(null, null, null, sphereGS.transform.localScale.x, "sphere");

                break;
            case GaussSrfType.cylinder:
                currentGaussSurfaceSize = new CurrentGSSize(cylinderGS.transform.localScale.x, null, null, sphereGS.transform.localScale.y, "cylinder");
                break;
            case GaussSrfType.cube:
                currentGaussSurfaceSize = new CurrentGSSize(cubeGS.transform.localScale.x, cubeGS.transform.localScale.y, cubeGS.transform.localScale.z, null, "cube");
                break;
            default:
                break;
        }

        CheckChargeSizeAndSurface();

    }
    public CurrentChargeSize currentChargeSize;
    private void UpdateChargeSize()
    {
        switch (chargeT)
        {
            case ChargeType.point:
                currentChargeSize = new CurrentChargeSize(radius: sphereGS.transform.localScale.x / 2f, name: "point");
                Debug.Log("Point!!!!!!!");
                break;
            case ChargeType.linear:
                currentChargeSize = new CurrentChargeSize(length: lineCharge.transform.localScale.y, name: "linear");
                Debug.Log("Linear!!!!");
                break;
            case ChargeType.planar:
                currentChargeSize = new CurrentChargeSize(width: planeCharge.transform.localScale.x, height: planeCharge.transform.localScale.y, name: "planar");
                Debug.Log("Planar!!!!");
                break;
            case ChargeType.noCharge:
                currentChargeSize = null; 
                break;
            default:
                break;
        }
    }

    private void CheckChargeSizeAndSurface()
    {
        if (currentChargeSize != null && currentGaussSurfaceSize != null)
        {
            bool shouldChangeMaterial = false;

            // For Sphere: Compare radius of the Gaussian surface with charge properties
            if (currentGaussSurfaceSize.getName() == "sphere")
            {
                // Compare the radius of the Gaussian surface with various charge properties
                if (currentGaussSurfaceSize.getRadius() <= currentChargeSize.getRadius() ||
                    currentGaussSurfaceSize.getRadius() <= currentChargeSize.getHeight() ||
                    currentGaussSurfaceSize.getRadius() <= currentChargeSize.getWidth() ||
                    currentGaussSurfaceSize.getRadius() <= currentChargeSize.getLength())
                {
                    shouldChangeMaterial = true;
                }
            }

            // For Cylinder: Compare radius and length of the Gaussian surface with charge properties
            else if (currentGaussSurfaceSize.getName() == "cylinder")
            {
                // Compare the radius and length of the Gaussian surface with the charge properties
                if (currentGaussSurfaceSize.getRadius() <= currentChargeSize.getRadius() ||
                    currentGaussSurfaceSize.getRadius() <= currentChargeSize.getHeight() ||
                    currentGaussSurfaceSize.getRadius() <= currentChargeSize.getWidth() ||
                    currentGaussSurfaceSize.getRadius() <= currentChargeSize.getLength() ||
                    currentGaussSurfaceSize.getLength() <= currentChargeSize.getLength())
                {
                    shouldChangeMaterial = true;
                }
            }

            // For Cube: Compare the dimensions of the cube with the charge properties
            else if (currentGaussSurfaceSize.getName() == "cube")
            {
                // Compare the length, width, and height of the Gaussian surface with the charge properties
                if (currentGaussSurfaceSize.getLength() <= currentChargeSize.getLength() ||
                    currentGaussSurfaceSize.getWidth() <= currentChargeSize.getWidth() ||
                    currentGaussSurfaceSize.getHeight() <= currentChargeSize.getHeight() ||
                    currentGaussSurfaceSize.getLength() <= currentChargeSize.getRadius())
                {
                    shouldChangeMaterial = true;
                }
            }

            // If the conditions are met to change the material
            //if (shouldChangeMaterial)
            //{
            //    // Change the material of the Gaussian surface (assuming sphere, cylinder, or cube)
            //    switch (GaussSrfT)
            //    {
            //        case GaussSrfType.sphere:
            //            sphereGS.GetComponent<Renderer>().material = smallChargeMaterial;
            //            break;
            //        case GaussSrfType.cylinder:
            //            cylinderGS.GetComponent<Renderer>().material = smallChargeMaterial;
            //            break;
            //        case GaussSrfType.cube:
            //            cubeGS.GetComponent<Renderer>().material = smallChargeMaterial;
            //            break;
            //        default:
            //            break;
            //    }
            //}
            //else
            //{
            //    // If no condition is met, restore the default material
            //    switch (GaussSrfT)
            //    {
            //        case GaussSrfType.sphere:
            //            sphereGS.GetComponent<Renderer>().material = defaultChargeMaterial;
            //            break;
            //        case GaussSrfType.cylinder:
            //            cylinderGS.GetComponent<Renderer>().material = defaultChargeMaterial;
            //            break;
            //        case GaussSrfType.cube:
            //            cubeGS.GetComponent<Renderer>().material = defaultChargeMaterial;
            //            break;
            //        default:
            //            break;
            //    }
            //}

            if (shouldChangeMaterial)
            {
                SetTransparencyGS(transparencyOriginalCharge);
                SetTransparencyCharge(transparencyOriginalGS);
            }
            else
            {
                SetTransparencyGS(transparencyOriginalGS);
                SetTransparencyCharge(transparencyOriginalCharge);

            }
        }
    }


    void OnRadiusSliderChanged(float value)
    {
        switch (GaussSrfT)
        {
            case GaussSrfType.sphere:
                sphereGS.transform.localScale = Vector3.one * initScale * value;
                sphereRadiusInput.text = value.ToString();
                break;
            case GaussSrfType.cylinder:
                cylinderGS.transform.localScale = new Vector3(initScale * value, cylinderGS.transform.localScale.y, initScale * value);
                cylinderRadiusInput.text = value.ToString();
                break;
            case GaussSrfType.cube:
                cubeGS.transform.localScale = new Vector3(cubeGS.transform.localScale.x, initScale * value, cubeGS.transform.localScale.z);
                cubeRadiusInput.text = value.ToString();
                break;
            default:
                break;
        }

        CheckChargeSizeAndSurface();
    }

    private void SetTransparencyGS(float alpha)
    {
        switch (GaussSrfT)
        {
            case GaussSrfType.sphere:
                SetMaterialTransparency(sphereGS, alpha);
                break;
            case GaussSrfType.cylinder:
                SetMaterialTransparency(cylinderGS, alpha);
                break;
            case GaussSrfType.cube:
                SetMaterialTransparency(cubeGS, alpha);
                break;
            default:
                break;
        }
    }

    private void SetTransparencyCharge(float alpha)
    {
        switch (chargeT)
        {
            case ChargeType.point:
                SetMaterialTransparency(sphereGS, alpha);
                break;
            case ChargeType.linear:
                SetMaterialTransparency(cylinderGS, alpha);
                break;
            case ChargeType.planar:
                SetMaterialTransparency(cubeGS, alpha);
                break;
            default:
                break;
        }
    }

    private void SetMaterialTransparency(GameObject obj, float alpha)
    {
        Renderer renderer = obj.GetComponent<Renderer>();
        if (renderer != null)
        {
            Material material = renderer.material;

            // Ensure the material uses transparency by setting the shader to one that supports transparency
            material.SetFloat("_Mode", 3);  // Set the mode to Transparent
            material.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.SrcAlpha);
            material.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
            material.SetInt("_ZWrite", 0);
            material.DisableKeyword("_ALPHATEST_ON");
            material.EnableKeyword("_ALPHABLEND_ON");
            material.DisableKeyword("_ALPHAPREMULTIPLY_ON");
            material.renderQueue = 3000;

            // Adjust the alpha of the material color to change transparency
            UnityEngine.Color color = material.color;
            color.a = alpha;
            material.color = color;
        }
        else
        {
            Debug.LogError("Renderer not found on the specified GameObject.");
        }
    }


    void OnRadiusInputChanged(string value)
    {
        if (float.TryParse(value, out float newSize))
        {
            newSize = Mathf.Clamp(newSize, 0, 10);

            switch (GaussSrfT)
            {
                case GaussSrfType.sphere:
                    sphereRadiusSlider.value = newSize;
                    break;
                case GaussSrfType.cylinder:
                    cylinderRadiusSlider.value = newSize;
                    break;
                case GaussSrfType.cube:
                    cubeRadiusSlider.value = newSize;
                    break;
                default:
                    break;
            }
        }
        else
        {
            Debug.LogError("Invalid input for size.");
        }
    }

    void OnLengthSliderChanged(float value)
    {
        switch (GaussSrfT)
        {
            case GaussSrfType.cylinder:
                cylinderGS.transform.localScale = new Vector3(cylinderGS.transform.localScale.x, initScale * value, cylinderGS.transform.localScale.z);
                cylinderLengthInput.text = value.ToString();
                break;
            case GaussSrfType.cube:
                cubeGS.transform.localScale = new Vector3(cubeGS.transform.localScale.x, initScale * value, initScale * value);
                cubeLengthInput.text = value.ToString();
                break;
            default:
                break;
        }
    }

    void OnLengthInputChanged(string value)
    {
        if (float.TryParse(value, out float newSize))
        {
            newSize = Mathf.Clamp(newSize, 0, 10);

            switch (GaussSrfT)
            {
                case GaussSrfType.cylinder:
                    cylinderLengthSlider.value = newSize;
                    break;
                case GaussSrfType.cube:
                    cubeLengthSlider.value = newSize;
                    break;
                default:
                    break;
            }
        }
        else
        {
            Debug.LogError("Invalid input for size.");
        }
    }

}
