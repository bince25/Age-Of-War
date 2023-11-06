using UnityEngine;
using UnityEngine.Networking;
using System.Collections;
using System;

public class HttpRequestHelper : MonoBehaviour
{
    public static HttpRequestHelper Instance { get; private set; }

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

    public void PostRequest(string url, string jsonContent, Action<string> onComplete, Action<string> onError)
    {
        StartCoroutine(PostRequestCoroutine(url, jsonContent, onComplete, onError));
    }

    private IEnumerator PostRequestCoroutine(string url, string jsonContent, Action<string> onComplete, Action<string> onError)
    {
        using (UnityWebRequest www = new UnityWebRequest(url, "POST"))
        {
            byte[] bodyRaw = System.Text.Encoding.UTF8.GetBytes(jsonContent);
            www.uploadHandler = new UploadHandlerRaw(bodyRaw);
            www.downloadHandler = new DownloadHandlerBuffer();
            www.SetRequestHeader("Content-Type", "application/json");

            // Place JWT token in the header
            string jwt_token = PlayerPrefs.GetString("accessToken");
            if (!string.IsNullOrEmpty(jwt_token))
            {
                www.SetRequestHeader("Authorization", "Bearer " + jwt_token);
            }

            yield return www.SendWebRequest();

            if (www.result == UnityWebRequest.Result.ConnectionError || www.result == UnityWebRequest.Result.ProtocolError)
            {
                onError?.Invoke(www.error);
            }
            else
            {
                // Extract the Set-Cookie header
                string cookieValue = www.GetResponseHeader("Set-Cookie");
                if (!string.IsNullOrEmpty(cookieValue))
                {
                    // Parse the cookie value to get the token
                    string tokenWithPrefix = cookieValue.Split(';')[0];

                    // Remove the "access_token=" prefix
                    string token = tokenWithPrefix.Replace("access_token=", "");
                    if (!string.IsNullOrEmpty(token) && token != "")
                        PlayerPrefs.SetString("accessToken", token);
                }

                onComplete?.Invoke(www.downloadHandler.text);
            }
        }
    }




}
