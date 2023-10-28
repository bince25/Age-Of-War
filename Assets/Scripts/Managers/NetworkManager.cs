using System;
using UnityEngine;

public class NetworkManager : MonoBehaviour
{
    public static NetworkManager Instance { get; private set; }

    public NetworkEnviroment networkEnviroment;
    [HideInInspector]
    public string baseApiUrl = "localhost:8080";

    [SerializeField]
    private string baseDevelopmentApiUrl = "localhost:8080";
    [SerializeField]
    private string baseProductionApiUrl = "167.172.105.111";

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            baseApiUrl = networkEnviroment == NetworkEnviroment.Development ? baseDevelopmentApiUrl : baseProductionApiUrl;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {

    }
}

[System.Serializable]
public enum NetworkEnviroment
{
    Development,
    Production
}