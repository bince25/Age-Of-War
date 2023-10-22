using System.Collections;
using UnityEngine;
using NativeWebSocket;
using System;

public class WebSocketClient : MonoBehaviour
{
    public static WebSocketClient Instance { get; private set; }

    private WebSocket websocket;

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

    public async void ConnectToWebSocket()
    {
        websocket = new WebSocket("ws://localhost:8080/game/ws");

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
            WebSocketEvents.Instance.HandleMessage(System.Text.Encoding.UTF8.GetString(bytes));
        };

        await websocket.Connect();
    }

    public async void SendWebSocketMessage(string message, Action action = null)
    {
        Debug.Log(websocket.State);
        Debug.Log($"Sending message: {message}");
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
