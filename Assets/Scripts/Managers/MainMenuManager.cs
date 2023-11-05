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

    public UIManager uiManager;

    private Stack<GameState> stateHistory = new Stack<GameState>();
    private GameState currentState;

    private void Awake()
    {
        modalController = modalControllerObject.GetComponent<ModalController>();
        ChangeState(GameState.Login);
    }

    // Method to change state with history tracking
    public void ChangeState(GameState newState, bool storeCurrentState = true)
    {
        if (storeCurrentState)
        {
            if (currentState != GameState.Login)
                stateHistory.Push(currentState);
        }

        currentState = newState;
        UpdateUIBasedOnState();
    }

    // Method to revert to the previous state
    public void GoBack()
    {
        if (stateHistory.Count > 0)
        {
            GameState previousState = stateHistory.Pop();
            // Don't store the current state since we are popping off the stack
            ChangeState(previousState, storeCurrentState: false);
        }
    }

    // Call this when you need to update the UI based on the new state
    private void UpdateUIBasedOnState()
    {
        Debug.Log(currentState);
        // Open the appropriate panel
        switch (currentState)
        {
            case GameState.Login:
                uiManager.ShowPanel(uiManager.loginPanel);
                break;
            case GameState.MainMenu:
                uiManager.ShowPanel(uiManager.mainMenuPanel);
                break;
            case GameState.GameModeSelect:
                uiManager.ShowPanel(uiManager.gameModePanel);
                break;
        }
    }


    public void Login()
    {
        AuthManager.Instance.LogInWithUsernamePassword(mail.text, password.text, () =>
        {
            Debug.Log("Logged in successfully!");
            ChangeState(GameState.MainMenu);
        }, () =>
        {
            Debug.LogError("Login failed!");
            modalController.ShowModal("Login failed!", "Please check your credentials and try again.");
        });
    }

    public void SelectGameMode()
    {
        ChangeState(GameState.GameModeSelect);
    }

    public void JoinGame()
    {
        ChangeState(GameState.JoinGame);
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
