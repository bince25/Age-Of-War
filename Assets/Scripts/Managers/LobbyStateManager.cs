using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
using System.Collections.Generic;
using System;
using Newtonsoft.Json;
using UnityEngine.UI;

public class LobbyStateManager : MonoBehaviour
{

    public static LobbyStateManager Instance { get; private set; }
    private string apiUrl;
    public string lobbyId;

    [SerializeField]
    private Button leftCountryButton;
    [SerializeField]
    private Button rightCountryButton;

    [SerializeField]
    private Button leftReadyButton;
    [SerializeField]
    private Button rightReadyButton;

    public Lobby lobby;

    public bool ready = false;
    public bool opponentReady = false;

    public TMP_InputField invitationCodeText;
    public Toggle publicToggle;

    public TMP_InputField invitationCodeInput;

    public Side side;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        if (NetworkManager.Instance.networkEnviroment == NetworkEnviroment.Development)
        {
            apiUrl = "http://" + NetworkManager.Instance.baseApiUrl + "/api/lobby";

        }
        else
        {
            // Change this to https
            apiUrl = "http://" + NetworkManager.Instance.baseApiUrl + "/api/lobby";
        }
    }

    private void HandlePlayerSide(Side side)
    {
        if (side == Side.Left)
        {
            leftCountryButton.interactable = true;
            leftReadyButton.interactable = true;
            rightCountryButton.interactable = false;
            rightReadyButton.interactable = false;

            leftCountryButton.GetComponentInChildren<TMP_Text>().text = "Select Country";
            rightCountryButton.GetComponentInChildren<TMP_Text>().text = "Opponent";

            leftReadyButton.GetComponentInChildren<TMP_Text>().text = "Ready";
            rightReadyButton.GetComponentInChildren<TMP_Text>().text = "Waiting for player...";
        }
        else
        {
            leftCountryButton.interactable = false;
            leftReadyButton.interactable = false;
            rightCountryButton.interactable = true;
            rightReadyButton.interactable = true;

            leftCountryButton.GetComponentInChildren<TMP_Text>().text = "Opponent";
            rightCountryButton.GetComponentInChildren<TMP_Text>().text = "Select Country";

            leftReadyButton.GetComponentInChildren<TMP_Text>().text = "Waiting for player...";
            rightReadyButton.GetComponentInChildren<TMP_Text>().text = "Ready";
        }
    }

    public void FindGame(Action onSuccess)
    {
        LoadingScreen.Instance.ShowLoadingPanel("Finding game...");
        HttpRequestHelper.Instance.PostRequest(apiUrl + "/find-and-join",
            $"{{}}",
            response =>
            {
                var jsonResponse = JsonConvert.DeserializeObject<ResponseData>(response);
                if (jsonResponse.success)
                {
                    Debug.Log(jsonResponse.data);
                    PlayerPrefs.SetString("lobbyId", jsonResponse.data.lobbyId);
                    PlayerPrefs.SetString("side", jsonResponse.data.side.ToString());
                    HandlePlayerSide(jsonResponse.data.side);
                    // Assuming the token is in the JSON response
                    lobbyId = PlayerPrefs.GetString("lobbyId");
                    onSuccess.Invoke();
                    LoadingScreen.Instance.HideLoadingPanel();
                }
                else
                {
                    Debug.LogError("Login failed: " + jsonResponse.message);
                    LoadingScreen.Instance.HideLoadingPanel();
                    ModalController.Instance.ShowModal("Error", jsonResponse.message);
                }
            },
            error =>
            {
                Debug.LogError("Network error: " + error);
                LoadingScreen.Instance.HideLoadingPanel();
                ModalController.Instance.ShowModal("Error", error);
            });
    }

    public void ConnectToLobby()
    {
        LoadingScreen.Instance.ShowLoadingPanel("Connecting to lobby...");
        WebSocketClient.Instance.ConnectToWebSocket(WebSocketType.Lobby);
        // wait for the connection to be established
        // then send the join lobby message
        WebSocketClient.Instance.AddOnOpenListener(() =>
        {
            JoinLobbyMessage message = new JoinLobbyMessage
            {
                action = "connectLobby",
                data = new JoinLobbyData { lobbyId = lobbyId },
                token = PlayerPrefs.GetString("accessToken")
            };

            string jsonMessage = JsonUtility.ToJson(message);

            WebSocketClient.Instance.SendWebSocketMessage(jsonMessage);
        });
    }

    public void CopyInvitationCode()
    {
        GUIUtility.systemCopyBuffer = invitationCodeText.text;

        ModalController.Instance.ShowModal("Copied!", "Invitation code copied to clipboard.");
    }

    public void PublicToggle()
    {
        PublicToggleMessage publicToggle = new PublicToggleMessage
        {
            action = "togglePrivate",
            data = new PublicToggleData
            {
                isPrivate = !lobby.isPrivate,
                lobbyId = lobbyId
            },
            token = PlayerPrefs.GetString("accessToken")
        };

        string jsonMessage = JsonConvert.SerializeObject(publicToggle);
        WebSocketClient.Instance.SendWebSocketMessage(jsonMessage);
    }


    public void HandleTogglePublic(bool isPrivate)
    {
        lobby.isPrivate = isPrivate;
        publicToggle.isOn = lobby.isPrivate;
    }

    public void ToggleReady()
    {

        PublicToggleMessage readyToggle = new PublicToggleMessage
        {
            action = "toggleReady",
            data = new PublicToggleData
            {
                isPrivate = !lobby.isPrivate,
                lobbyId = lobbyId
            },
            token = PlayerPrefs.GetString("accessToken")
        };

        string jsonMessage = JsonConvert.SerializeObject(readyToggle);
        WebSocketClient.Instance.SendWebSocketMessage(jsonMessage);

    }
    public void HandleToggleReady()
    {
        leftReadyButton.GetComponentInChildren<TMP_Text>().text = ready ? "Unready" : "Ready";
        rightReadyButton.GetComponentInChildren<TMP_Text>().text = opponentReady ? "Unready" : "Ready";
    }

    public void LeaveLobby()
    {

        LeaveLobbyMessage leaveLobby = new LeaveLobbyMessage
        {
            action = "leaveLobby",
            data = new LeaveLobbyData
            {
                lobbyId = lobbyId
            },
            token = PlayerPrefs.GetString("accessToken")
        };

        string jsonMessage = JsonConvert.SerializeObject(leaveLobby);
        WebSocketClient.Instance.SendWebSocketMessage(jsonMessage);

    }
    public void HandleLeaveLobby(Side playerSide)
    {
        if (side == playerSide)
        {
            lobby = null;
            UIManager.Instance.lobbyPanel.SetActive(false);
        }

    }

    public void JoinGameWithInvitationCode(Action onSuccess)
    {
        string invitationCode = invitationCodeInput.text;

        LoadingScreen.Instance.ShowLoadingPanel("Finding game...");
        HttpRequestHelper.Instance.PostRequest(apiUrl + "/join-with-code",
            $"{{\"invitationCode\":\"{invitationCode}\"}}",
            response =>
            {
                var jsonResponse = JsonConvert.DeserializeObject<ResponseData>(response);
                if (jsonResponse.success)
                {
                    Debug.Log(jsonResponse.data);
                    PlayerPrefs.SetString("lobbyId", jsonResponse.data.lobbyId);
                    PlayerPrefs.SetString("side", jsonResponse.data.side.ToString());
                    HandlePlayerSide(jsonResponse.data.side);
                    // Assuming the token is in the JSON response
                    lobbyId = PlayerPrefs.GetString("lobbyId");
                    Debug.Log("Found game!");
                    onSuccess.Invoke();
                    LoadingScreen.Instance.HideLoadingPanel();
                }
                else
                {
                    Debug.LogError("Login failed: " + jsonResponse.message);
                    LoadingScreen.Instance.HideLoadingPanel();
                    ModalController.Instance.ShowModal("Error", jsonResponse.message);
                }
            },
            error =>
            {
                Debug.LogError("Network error: " + error);
                LoadingScreen.Instance.HideLoadingPanel();
                ModalController.Instance.ShowModal("Error", error);
            });
    }


}

[System.Serializable]
public class Lobby
{
    public string id;
    public string name;
    public string ownerId;
    public bool isPrivate;
    public string password;
    public string gameSessionId;
    public int maxPlayers;
    public int currentPlayers;
    public string invitationCode;
}

[System.Serializable]
public class ResponseData
{
    public bool success;
    public string message;
    public LobbyResponseData data;
}

public class LobbyResponseData
{
    public string lobbyId;

    public Side side;
}

[System.Serializable]
public class JoinLobbyMessage
{
    public string action;
    public JoinLobbyData data;
    public string token;
}

[System.Serializable]
public class JoinLobbyData
{
    public string lobbyId;
}

[System.Serializable]
public class ReadyToggleMessage
{
    public string action;
    public ReadyToggleData data;
    public string token;
}

[System.Serializable]
public class ReadyToggleData
{
    public bool ready;
    public int side;
    public string lobbyId;
}

[System.Serializable]
public class PublicToggleMessage
{
    public string action;
    public PublicToggleData data;
    public string token;
}

[System.Serializable]
public class PublicToggleData
{
    public bool isPrivate;
    public string lobbyId;
}

[System.Serializable]
public class JoinLobbyInviteMessage
{
    public string action;
    public JoinLobbyInviteData data;
    public string token;
}

[System.Serializable]
public class JoinLobbyInviteData
{
    public string invitationCode;
}

[System.Serializable]
public class LeaveLobbyMessage
{
    public string action;
    public LeaveLobbyData data;
    public string token;
}

[System.Serializable]
public class LeaveLobbyData
{
    public string lobbyId;
}