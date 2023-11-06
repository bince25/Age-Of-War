using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
using System.Collections.Generic;
using System;
using Newtonsoft.Json;
using UnityEngine.UI;

public class CountryManager : MonoBehaviour
{

    public static CountryManager Instance { get; private set; }
    private string apiUrl;

    private Image banner;

    [SerializeField]
    private Image leftBanner;
    [SerializeField]
    private Image rightBanner;

    [SerializeField]
    private TMP_Text leftCountryText;
    [SerializeField]
    private TMP_Text rightCountryText;

    [SerializeField]
    private Sprite englandBanner;

    [SerializeField]
    private Sprite franceBanner;

    [SerializeField]
    private Sprite romeBanner;

    [SerializeField]
    private Sprite vikingsBanner;

    public Country country;

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

    }

    public void SelectEngland()
    {
        country = Country.England;
        SendCountryUpdate(country);
    }

    public void SelectFrance()
    {
        country = Country.France;
        SendCountryUpdate(country);
    }

    public void SelectRome()
    {
        country = Country.Rome;
        SendCountryUpdate(country);
    }

    public void SelectVikings()
    {
        country = Country.Vikings;
        SendCountryUpdate(country);
    }

    private void SendCountryUpdate(Country country)
    {
        Debug.Log((int)country);
        UpdateCountryMessage updateCountryMessage = new UpdateCountryMessage
        {
            action = "selectCountry",
            data = new UpdateCountryData
            {
                lobbyId = PlayerPrefs.GetString("lobbyId"),
                country = (int)country,
                side = SpawnSide.Left
            },
            token = PlayerPrefs.GetString("accessToken")
        };

        string jsonMessage = JsonConvert.SerializeObject(updateCountryMessage);
        WebSocketClient.Instance.SendWebSocketMessage(jsonMessage);

        UIManager.Instance.ClosePanel(UIManager.Instance.countryPanel);
    }

    public void HandleCountryUpdate(Country country)
    {
        if (banner == null)
        {
            Side side = PlayerPrefs.GetString("side") == "Left" ? Side.Left : Side.Right;
            banner = side == Side.Left ? leftBanner : rightBanner;
        }

        this.country = country;
        switch (country)
        {
            case Country.England:
                banner.sprite = englandBanner;
                break;
            case Country.France:
                banner.sprite = franceBanner;
                break;
            case Country.Rome:
                banner.sprite = romeBanner;
                break;
            case Country.Vikings:
                banner.sprite = vikingsBanner;
                break;
        }
    }

    public void ShowCountrySelectionPanel()
    {
        UIManager.Instance.countryPanel.SetActive(true);
    }
}

[System.Serializable]
public class UpdateCountryMessage
{
    public string action;
    public UpdateCountryData data;
    public string token;
}

[System.Serializable]
public class UpdateCountryData
{
    public string lobbyId;
    public int country;
    public SpawnSide side;
}