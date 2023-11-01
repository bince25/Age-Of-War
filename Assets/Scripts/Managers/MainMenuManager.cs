using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenuManager : MonoBehaviour
{
    public GameObject loginPanel, mainmenuPanel, registerPanel, settingsPanel, countriesPanel, helpPanel, countryInfoPanel;

    public void Login()
    {
        Debug.Log("Login Successful");
        loginPanel.SetActive(false);
        mainmenuPanel.SetActive(true);
    }
    public void OpenCountriesPanel()
    {
        countriesPanel.SetActive(true);
        mainmenuPanel.SetActive(false);
    }
    public void OpenSettingsPanel()
    {
        settingsPanel.SetActive(true);
        mainmenuPanel.SetActive(false);
    }
    public void OpenCountryInfoPanel()
    {
        countryInfoPanel.SetActive(true);
        countriesPanel.SetActive(false);
    }
    public void Play()
    {
        Debug.Log("Play");
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }
    public void Exit()
    {
        Application.Quit();
    }
    public void BackToMainMenu()
    {
        countriesPanel.SetActive(false);
        settingsPanel.SetActive(false);
        mainmenuPanel.SetActive(true);
    }
}
