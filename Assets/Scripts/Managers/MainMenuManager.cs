using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenuManager : MonoBehaviour
{
    public GameObject loginPanel, mainmenuPanel, registerPanel, settingsPanel, countriesPanel, helpPanel, countryInfoPanel;

    public TMP_InputField mail, password;

    public GameObject modalControllerObject;
    private ModalController modalController;
    [SerializeField]
    private LoadingScreen loadingScreen;

    private void Awake()
    {
        modalController = modalControllerObject.GetComponent<ModalController>();
    }

    public void Login()
    {
        AuthManager.Instance.LogInWithUsernamePassword(mail.text, password.text, () =>
        {
            Debug.Log("Logged in successfully!");
            loginPanel.SetActive(false);
            mainmenuPanel.SetActive(true);
        }, () =>
        {
            Debug.LogError("Login failed!");
            modalController.ShowModal("Login failed!", "Please check your credentials and try again.");
        });
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
        loadingScreen.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
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
