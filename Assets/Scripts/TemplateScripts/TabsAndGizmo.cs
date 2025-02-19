//using System.Collections;
//using System.Collections.Generic;
//using UnityEngine;
//using UnityEngine.UI;
//using TMPro;



//public class Tabs : MonoBehaviour
//{
//    // Settings Panel
//    [SerializeField] private GameObject chargePanel;
//    //[SerializeField] private GameObject gaussianPanel;
//    [SerializeField] private GameObject dimensionGSParentPanel;

//    [SerializeField] private GameObject credits;

//    [SerializeField] private Button settingButton;
 
//    [SerializeField] private Button gizmoButton;
//    [SerializeField] private TextMeshProUGUI gizmoButtonText;

//    private bool isEFieldTab = true;
//    private bool isGizmo = true;
//    [SerializeField] private GameObject gizmo;
//    [SerializeField] private GameObject gizmoButtonOnOff;

//    // Credits Panel
//    [SerializeField] private Button creditsButton;
//    [SerializeField] private GameObject creditsPanel;
//    [SerializeField] private bool isShown = false;


//    //for writing a random name order

//    [SerializeField] private TextMeshProUGUI nameLabel;
//    [SerializeField] private Button nameEasterEgg;

//    // Start is called before the first frame update

//    public float mobileScale = 10f;
//    public float pcScale = 1f;
//    void Start()
//    {


//    void OnGizmoButtonClicked()
//    {
//        if (isGizmo)
//        {
//            gizmo.SetActive(false);
//            isGizmo = false;
//            gizmoButtonText.text = "Off";
//        }
//        else
//        {
//            gizmo.SetActive(true);
//            isGizmo = true;
//            gizmoButtonText.text = "On";
//        }
//    }
//    void OnTabClicked()
//    {
//        if (chargePanel != null)
//        {
//            if (isEFieldTab)
//            {
//                {
//                    chargePanel.SetActive(false);
//                    isEFieldTab = false;
//                    //gaussianPanel.SetActive(false);
//                    credits.SetActive(false);
//                    gizmoButtonOnOff.SetActive(false);
//                    dimensionGSParentPanel.SetActive(false);
//                }

//            }
//            else
//            {
//                chargePanel.SetActive(true);
//                isEFieldTab = true;
//                //gaussianPanel.SetActive(true);
//                credits.SetActive(true);
//                gizmoButtonOnOff.SetActive(true);
//                dimensionGSParentPanel.SetActive(true) ;

//            }
//        }
//    }
//    int easterEggCount = 0;

//    private void OnNamesClicked() //EasterEgg
//    {
//        easterEggCount++;
//        int TARGET_COUNT = 5;

//        if(easterEggCount == TARGET_COUNT)
//        {
//            EasterEgg();
//            easterEggCount = 0;
//        }
        
//    }
//    private void EasterEgg() //Currently just changes the order of the names
//    {
//        nameLabel.text = RandomNameString();
//    }
//    [SerializeField] private Canvas canvas;
//    private void OnCreditsClicked()
//    {
//        Debug.Log("Credit button clicked. isPlaying: " + isShown);
//        if (isShown)
//        {
//            creditsPanel.SetActive(false);
//            isShown = false;
//            //canvas.GetComponent<GraphicRaycaster>().enabled = true;

//        }
//        else
//        {
//            isShown = true;
//            creditsPanel.SetActive(true);
//            //canvas.GetComponent<GraphicRaycaster>().enabled = false;



//        }
//    }

//    private string RandomNameString()
//    {
//        List<string> list = new List<string> { "Tamara", "Davey", "Suryash", "Liyu", "Sabrina", "Ryan <b>Tapping</b> (5)", "Phil Krasicky" };
//        int listSize = list.Count;
//        string nameText = "";
//        List<string> ourList = new List<string>();
//        if(list.Count > 0)
//        {
//            for (int i = 0; i < listSize; i++)
//            {
//                int randInt = Random.Range(0, list.Count);
//                string name = list[randInt];

//                ourList.Add(name);
//                list.RemoveAt(randInt);
//            }
//        }

//        nameText = string.Join(", ", ourList);
//        Debug.Log(ourList);
//        return nameText;
//    }

//    // Update is called once per frame
//    void Update()
//        {

//        }
//    }

