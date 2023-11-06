using System;
using UnityEngine;

public class AuthManager : MonoBehaviour
{
    public static AuthManager Instance { get; private set; }

    [SerializeField]
    private LoadingScreen loadingScreen;

    private string apiUrl;
    private string accessToken;

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
            apiUrl = "http://" + NetworkManager.Instance.baseApiUrl + "/api/auth";

        }
        else
        {
            // Change this to https
            apiUrl = "http://" + NetworkManager.Instance.baseApiUrl + "/api/auth";
        }
    }

    public void DefaultLogin(Action onSuccess)
    {
        string username = "menes";
        string password = "12345";

        HttpRequestHelper.Instance.PostRequest(apiUrl + "/login",
            $"{{\"username\":\"{username}\",\"password\":\"{password}\"}}",
            response =>
            {
                var jsonResponse = JsonUtility.FromJson<ResponseData>(response);
                if (jsonResponse.success)
                {
                    PlayerPrefs.SetString("accessToken", jsonResponse.data.token);
                    // Assuming the token is in the JSON response
                    accessToken = PlayerPrefs.GetString("accessToken");
                    Debug.Log("Logged in successfully!");
                    onSuccess.Invoke();
                }
                else
                {
                    Debug.LogError("Login failed: " + jsonResponse.message);
                }
            },
            error =>
            {
                Debug.LogError("Network error: " + error);
            });
    }

    public void LogInWithUsernamePassword(string username, string password, Action onSuccess, Action onFail)
    {
        loadingScreen.ShowLoadingPanel("Logging in...");
        HttpRequestHelper.Instance.PostRequest(apiUrl + "/login",
            $"{{\"username\":\"{username}\",\"password\":\"{password}\"}}",
            response =>
            {
                var jsonResponse = JsonUtility.FromJson<ResponseData>(response);
                if (jsonResponse.success)
                {
                    Debug.Log("Logged in successfully! token: ");
                    onSuccess.Invoke();
                    loadingScreen.HideLoadingPanel();
                }
                else
                {
                    Debug.LogError("Login failed: " + jsonResponse.message);
                    onFail.Invoke();
                    loadingScreen.HideLoadingPanel();
                }
            },
            error =>
            {
                Debug.LogError("Network error: " + error);
                onFail.Invoke();
                loadingScreen.HideLoadingPanel();
            });
    }

    public bool IsLoggedIn()
    {
        return !string.IsNullOrEmpty(accessToken);
    }


    [System.Serializable]
    public class ResponseData
    {
        public bool success;
        public string message;
        public TokenData data;
    }

    [System.Serializable]
    public class TokenData
    {
        public string token;
    }
}
