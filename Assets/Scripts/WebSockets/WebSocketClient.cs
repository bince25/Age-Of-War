using System.Collections;
using UnityEngine;
using NativeWebSocket;
using System;

public class WebSocketClient : MonoBehaviour
{
    private string websocketUrl;

    public static WebSocketClient Instance { get; private set; }

    private WebSocket websocket;
    private string baseUrl;

    private void Awake()
    {
        // Singleton pattern
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
            return;
        }
    }

    private void Start()
    {
        if (NetworkManager.Instance.networkEnviroment == NetworkEnviroment.Development)
        {
            websocketUrl = "ws://" + NetworkManager.Instance.baseApiUrl + "/game/ws";
            baseUrl = "ws://" + NetworkManager.Instance.baseApiUrl;
        }
        else
        {
            // Change this to wss
            websocketUrl = "ws://" + NetworkManager.Instance.baseApiUrl + "/game/ws";
            baseUrl = "ws://" + NetworkManager.Instance.baseApiUrl;
        }
    }

    public async void ConnectToWebSocket()
    {
        websocket = new WebSocket(websocketUrl);

        websocket.OnOpen += () =>
        {
            Debug.Log("Connection open!");
        };

        websocket.OnError += (e) =>
        {
            Debug.Log($"Error occurred: {e}");
        };

        websocket.OnClose += (e) =>
        {
            Debug.Log($"Connection closed! Code: {e}");
        };

        websocket.OnMessage += (bytes) =>
        {
            Debug.Log($"OnMessage!");
            WebSocketEvents.Instance.HandleGameMessage(System.Text.Encoding.UTF8.GetString(bytes));
        };

        await websocket.Connect();
    }

    public void AddOnOpenListener(Action action)
    {
        websocket.OnOpen += () =>
        {
            action.Invoke();
        };
    }

    public async void ConnectToWebSocket(WebSocketType type)
    {
        string url = "";
        switch (type)
        {
            case WebSocketType.Game:
                url = baseUrl + "/game/ws";
                break;
            case WebSocketType.Lobby:
                url = baseUrl + "/lobby/ws";
                break;
        }
        websocket = new WebSocket(url);

        websocket.OnOpen += () =>
        {
            Debug.Log("Connection open!");
        };

        websocket.OnError += (e) =>
        {
            Debug.Log($"Error occurred: {e}");
        };

        websocket.OnClose += (e) =>
        {
            Debug.Log($"Connection closed! Code: {e}");
        };

        websocket.OnMessage += (bytes) =>
        {
            Debug.Log($"OnMessage!");

            if (type == WebSocketType.Game)
            {
                WebSocketEvents.Instance.HandleGameMessage(System.Text.Encoding.UTF8.GetString(bytes));
            }
            else if (type == WebSocketType.Lobby)
            {
                WebSocketEvents.Instance.HandleLobbyMessage(System.Text.Encoding.UTF8.GetString(bytes));
            }
        };

        await websocket.Connect();
    }

    public async void SendWebSocketMessage(string message, Action action = null)
    {
        // Debug.Log(websocket.State);
        // Debug.Log($"Sending message: {message}");
        await websocket.SendText(message);

        if (action != null)
        {
            action.Invoke();
        }
    }

    private void Update()
    {
        if (websocket != null)
        {
#if !UNITY_WEBGL || UNITY_EDITOR
            websocket.DispatchMessageQueue();
#endif
        }
    }

    private async void OnApplicationQuit()
    {
        if (websocket != null) await websocket.Close();
    }
}


