using System;
using UnityEngine;

public class AuthManager : MonoBehaviour
{
    public static AuthManager Instance { get; private set; }

    private string apiUrl = "http://localhost:8080/api/auth";
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
