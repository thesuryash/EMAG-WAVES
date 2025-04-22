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
    [Header("Charge and Gaussian Surfaces")]
    [SerializeField] public GameObject pointCharge;
    [SerializeField] public GameObject lineCharge;
    [SerializeField] public GameObject planeCharge;
    [SerializeField] public GameObject sphereGS;
    [SerializeField] public GameObject cylinder1GS;
    [SerializeField] public GameObject cylinder2GS;
    [SerializeField] public GameObject cubeGS;

    [Header("Charge Toggles")]
    [SerializeField] public Toggle noChargeToggle;
    [SerializeField] public Toggle pointChargeToggle;
    [SerializeField] public Toggle lineChargeToggle;
    [SerializeField] public Toggle planeChargeToggle;

    [Header("GS Toggles")]
    [SerializeField] public Toggle noGSToggle;
    [SerializeField] public Toggle sphereGSToggle;
    [SerializeField] public Toggle cylinderGSToggle;
    [SerializeField] public Toggle cubeGSToggle;

    [Header("Sphere GS Controls")]
    [SerializeField] public GameObject SphereDimensions;
    [SerializeField] public Slider sphereRadiusSlider;
    [SerializeField] public TMP_InputField sphereRadiusInput;

    [Header("Cylinder 1 GS Controls")]
    [SerializeField] public GameObject Cylinder1Dimensions;
    [SerializeField] public Slider cylinder1RadiusSlider;
    [SerializeField] public Slider cylinder1LengthSlider;
    [SerializeField] public TMP_InputField cylinder1RadiusInput;
    [SerializeField] public TMP_InputField cylinder1LengthInput;

    [Header("Cylinder 2 GS Controls")]
    [SerializeField] public GameObject Cylinder2Dimensions;
    [SerializeField] public Slider cylinder2RadiusSlider;
    [SerializeField] public Slider cylinder2LengthSlider;
    [SerializeField] public TMP_InputField cylinder2RadiusInput;
    [SerializeField] public TMP_InputField cylinder2LengthInput;

    [Header("Cube GS Controls")]
    [SerializeField] public GameObject CubeDimensions;
    [SerializeField] public Slider cubeRadiusSlider;
    [SerializeField] public Slider cubeAreaSlider;
    [SerializeField] public TMP_InputField cubeRadiusInput;
    [SerializeField] public TMP_InputField cubeAreaInput;

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
        noSrf, sphere, cylinder1, cylinder2, cube
    }

    public ChargeType chargeT = ChargeType.point;
    public GaussSrfType GaussSrfT = GaussSrfType.noSrf;




    private void Awake()
    {
        gsRadius = new (Slider, TMP_InputField)[] { (sphereRadiusSlider, sphereRadiusInput), (cylinder1RadiusSlider, cylinder1RadiusInput), (cylinder2RadiusSlider, cylinder2RadiusInput), (cubeRadiusSlider, cubeRadiusInput) };
        gsLength = new (Slider, TMP_InputField)[] { (cylinder1LengthSlider, cylinder1LengthInput), (cylinder2LengthSlider, cylinder2LengthInput), (cubeAreaSlider, cubeAreaInput) };

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
        cylinderGSToggle.onValueChanged.AddListener(delegate { OnCylinder1GSToggleOn(); });
        cylinderGSToggle.onValueChanged.AddListener(delegate { OnCylinder2GSToggleOn(); });
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

    void OnCylinder1GSToggleOn()
    {
        GaussSrfT = GaussSrfType.cylinder1;
        UpdateActiveObject();
    }

    void OnCylinder2GSToggleOn()
    {
        GaussSrfT = GaussSrfType.cylinder2;
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
        cylinder1GS.SetActive(GaussSrfT == GaussSrfType.cylinder1);
        cylinder2GS.SetActive(GaussSrfT == GaussSrfType.cylinder2);
        cubeGS.SetActive(GaussSrfT == GaussSrfType.cube);

        SphereDimensions.SetActive(GaussSrfT == GaussSrfType.sphere);
        Cylinder1Dimensions.SetActive(GaussSrfT == GaussSrfType.cylinder1);
        Cylinder2Dimensions.SetActive(GaussSrfT == GaussSrfType.cylinder2);
        CubeDimensions.SetActive(GaussSrfT == GaussSrfType.cube);


        switch (GaussSrfT)
        {
            case GaussSrfType.sphere:
                currentGaussSurfaceSize = new CurrentGSSize(null, null, null, sphereGS.transform.localScale.x, "sphere");

                break;
            case GaussSrfType.cylinder1:
                currentGaussSurfaceSize = new CurrentGSSize(cylinder1GS.transform.localScale.x, null, null, sphereGS.transform.localScale.y, "cylinder1");
                break;
            case GaussSrfType.cylinder2:
                currentGaussSurfaceSize = new CurrentGSSize(cylinder2GS.transform.localScale.x, null, null, sphereGS.transform.localScale.y, "cylinder2");
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
            else if (currentGaussSurfaceSize.getName() == "cylinder1")
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

            else if (currentGaussSurfaceSize.getName() == "cylinder2")
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
            case GaussSrfType.cylinder1:
                cylinder1GS.transform.localScale = new Vector3(initScale * value, cylinder1GS.transform.localScale.y, initScale * value);
                cylinder1RadiusInput.text = value.ToString();
                cylinder1RadiusInput.text = value.ToString();
                break;
            case GaussSrfType.cylinder2:
                cylinder1GS.transform.localScale = new Vector3(initScale * value, cylinder1GS.transform.localScale.y, initScale * value);
                cylinder2RadiusInput.text = value.ToString();
                cylinder2RadiusInput.text = value.ToString();
                break;
            case GaussSrfType.cube:
                cubeGS.transform.localScale = new Vector3( cubeGS.transform.localScale.x, initScale * value, cubeGS.transform.localScale.z);
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
            case GaussSrfType.cylinder1:
                SetMaterialTransparency(cylinder1GS, alpha);
                break;
            case GaussSrfType.cylinder2:
                SetMaterialTransparency(cylinder1GS, alpha);
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
                SetMaterialTransparency(cylinder1GS, alpha);
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
                case GaussSrfType.cylinder1:
                    cylinder1RadiusSlider.value = newSize;
                    break;
                case GaussSrfType.cylinder2:
                    cylinder2RadiusSlider.value = newSize;
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
            case GaussSrfType.cylinder1:
                cylinder1GS.transform.localScale = new Vector3(cylinder1GS.transform.localScale.x, initScale * value, cylinder1GS.transform.localScale.z);
                cylinder1LengthInput.text = value.ToString();
                break;
            case GaussSrfType.cylinder2:
                cylinder1GS.transform.localScale = new Vector3(cylinder1GS.transform.localScale.x, initScale * value, cylinder1GS.transform.localScale.z);
                cylinder2LengthInput.text = value.ToString();
                break;
            case GaussSrfType.cube:
                cubeGS.transform.localScale = new Vector3(initScale * value, cubeGS.transform.localScale.y, initScale * value);
                cubeAreaInput.text = value.ToString();
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
                case GaussSrfType.cylinder1:
                    cylinder1LengthSlider.value = newSize;
                    break;
                case GaussSrfType.cylinder2:
                    cylinder2LengthSlider.value = newSize;
                    break;
                case GaussSrfType.cube:
                    cubeAreaSlider.value = newSize;
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
