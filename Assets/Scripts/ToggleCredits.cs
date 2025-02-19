using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using static Tabs;

public class ToggleCredits : MonoBehaviour
{
    [SerializeField] private Button creditsButton;
    [SerializeField] private GameObject creditsPanel;
    [SerializeField] private bool isShown = false;

    // Start is called before the first frame update
    void Start()
    {
        creditsButton.onClick.AddListener(OnCreditsClicked);
        creditsPanel.SetActive(false);
    }

    private void OnCreditsClicked()
    {
        Debug.Log("Credit button clicked. isPlaying: " + isShown);
        if (!isShown)
        {
            isShown = true;
            creditsPanel.SetActive(true);

        }
        else
        {
            isShown = false;
            creditsPanel.SetActive(false);

        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
