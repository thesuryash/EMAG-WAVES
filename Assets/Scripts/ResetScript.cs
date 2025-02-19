using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class ResetScript : MonoBehaviour
{
    [SerializeField] private Button yesButton;
    [SerializeField] private Button noButton;
    [SerializeField] private GameObject confirmationPanel;
    [SerializeField] private Button resetButton;
    [SerializeField] private GameObject resetButtonObject;

    //private bool isConfirmationOn = false;
    private Vector3 initPos;
    // Start is called before the first frame update

    private void Awake()
    {
        initPos = resetButtonObject.transform.position;
        confirmationPanel.SetActive(false);
        resetButton.onClick.AddListener(OnResetButtonClicked);
        yesButton.onClick.AddListener(OnYesButtonClicked);
        noButton.onClick.AddListener(OnNoButtonClicked);
    }
    void Start()
    {
        //resetButton.gameObject.SetActive(true);

        //confirmationPanel.SetActive(false);
    }

    void OnResetButtonClicked()
    {

        //if (!isConfirmationOn)
        //{
        //Destroy(resetButtonObject);
        //resetButton.gameObject.SetActive(false);
        //resetButtonObject.SetActive(false);
        //resetButtonObject.transform.position = new Vector3 (100000, 0, 0);
            resetButtonObject.SetActive(false);
        Debug.Log("DONE turning off the reset button");
            confirmationPanel.SetActive(true);
            //Time.timeScale = 0f;
            //isConfirmationOn = true;
        //}
        Debug.Log("********************************************RESET CLICKED");
        
    }


    void OnYesButtonClicked()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name); 
        //isConfirmationOn = false;

    }

    void OnNoButtonClicked()
    {
        confirmationPanel.SetActive(false);
        resetButtonObject.SetActive(true);
        //resetButtonObject.transform.position = initPos;
        //isConfirmationOn = false;
    }

    // Update is called once per frame
    void Update()
    {

        //resetButton.onClick.AddListener(OnResetButtonClicked);
        //yesButton.onClick.AddListener(OnYesButtonClicked);
        //noButton.onClick.AddListener(OnNoButtonClicked);
    }
}
