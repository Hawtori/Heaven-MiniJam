using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class MenuButtons : MonoBehaviour
{
    public TMP_Text settingsText;

    public void StartGame()
    {
        SceneManager.LoadScene(1);
    }

    public void ExitGame()
    {
        Application.Quit();
    }

    public void Settings()
    {
        settingsText.text = "Sorry, no settings yet :(";
        Invoke("DisableSettings", 2f);
    }

    private void DisableSettings()
    {
        settingsText.text = "";
    }
}
