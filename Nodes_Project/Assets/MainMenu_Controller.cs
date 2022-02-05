using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DataSystem;
using UnityEngine.SceneManagement;

public class MainMenu_Controller : MonoBehaviour
{
    public Button continueButton;

    public GameObject areYouSurePanel;

    private void Awake()
    {
        areYouSurePanel.SetActive(false);

        var data = StaticData.Data;

        if(!data.IsGameInProgress())
        {
            continueButton.gameObject.SetActive(false);
        }
    }

    public void TryPlayButton()
    {
        var data = StaticData.Data;

        if (data.IsGameInProgress())
        {
            areYouSurePanel.SetActive(true);
        }
        else
        {
            SceneManager.LoadScene("Select Mode Scene");
        }
    }

}
