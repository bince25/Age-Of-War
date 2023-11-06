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

    private Stack<GameStateEnum> stateHistory = new Stack<GameStateEnum>();
    private GameStateEnum currentState;
    [SerializeField]
    private GameObject returnButton;

    private void Awake()
    {
        modalController = modalControllerObject.GetComponent<ModalController>();
        ChangeState(GameStateEnum.Login);
    }

    // Method to change state with history tracking
    public void ChangeState(GameStateEnum newState, bool storeCurrentState = true)
    {
        if (storeCurrentState)
        {
            if (currentState != GameStateEnum.Login)
                stateHistory.Push(currentState);
        }

        currentState = newState;
        UpdateUIBasedOnState();
        CheckStateHistory();
    }

    // Method to revert to the previous state
    public void GoBack()
    {
        if (stateHistory.Count > 0)
        {
            GameStateEnum previousState = stateHistory.Pop();
            // Don't store the current state since we are popping off the stack
            ChangeState(previousState, storeCurrentState: false);
        }
    }

    public void CheckStateHistory()
    {
        if (stateHistory.Count > 0)
        {
            returnButton.SetActive(true);
        }
        else
        {
            returnButton.SetActive(false);
        }
    }

    // Call this when you need to update the UI based on the new state
    private void UpdateUIBasedOnState()
    {
        Debug.Log(currentState);
        // Open the appropriate panel
        switch (currentState)
        {
            case GameStateEnum.Login:
                uiManager.ShowPanel(uiManager.loginPanel);
                break;
            case GameStateEnum.MainMenu:
                uiManager.ShowPanel(uiManager.mainMenuPanel);
                break;
            case GameStateEnum.GameModeSelect:
                uiManager.ShowPanel(uiManager.gameModePanel);
                break;
            case GameStateEnum.JoinGame:
                uiManager.ShowPanel(uiManager.joinGamePanel);
                break;
            case GameStateEnum.Lobby:
                LobbyStateManager.Instance.ConnectToLobby();
                break;
        }
    }


    public void Login()
    {
        AuthManager.Instance.LogInWithUsernamePassword(mail.text, password.text, () =>
        {
            Debug.Log("Logged in successfully!");
            ChangeState(GameStateEnum.MainMenu);
        }, () =>
        {
            Debug.LogError("Login failed!");
            modalController.ShowModal("Login failed!", "Please check your credentials and try again.");
        });
    }

    public void SelectGameMode()
    {
        ChangeState(GameStateEnum.GameModeSelect);
    }

    public void JoinGameScreen()
    {
        ChangeState(GameStateEnum.JoinGame);
    }

    public void FindGame()
    {
        LobbyStateManager.Instance.FindGame(() =>
        {
            Debug.Log("Found game!");
            loadingScreen.HideLoadingPanel();
            ChangeState(GameStateEnum.Lobby);
        });
    }

    public void JoinGameWithInvitationCode()
    {
        LobbyStateManager.Instance.JoinGameWithInvitationCode(() =>
        {
            Debug.Log("Found game!");
            loadingScreen.HideLoadingPanel();
            ChangeState(GameStateEnum.Lobby);
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
