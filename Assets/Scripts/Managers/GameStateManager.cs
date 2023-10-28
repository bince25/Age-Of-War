using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
using System.Collections.Generic;

public class GameStateManager : MonoBehaviour
{
    private GameObject leftCastle, rightCastle;
    [SerializeField]
    private GameObject gameOverPanel;
    [SerializeField]
    private GameObject leftWinText, rightWinText;

    private TextMeshProUGUI ageGoalText;
    [SerializeField]
    public Ages leftAge, rightAge;

    public SpawnSide playerSide = SpawnSide.Left;

    public static GameStateManager Instance { get; private set; }

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

    void Start()
    {
        leftCastle = GameObject.FindGameObjectWithTag("LeftCastle");
        rightCastle = GameObject.FindGameObjectWithTag("RightCastle");
    }

    // Update is called once per frame
    void Update()
    {
        if (leftCastle.GetComponent<Castle>().currentHealth <= 0)
        {
            leftCastle.GetComponent<Castle>().currentHealth = 0;
            gameOverPanel.SetActive(true);
            rightWinText.SetActive(true);
        }
        else if (rightCastle.GetComponent<Castle>().currentHealth <= 0)
        {
            rightCastle.GetComponent<Castle>().currentHealth = 0;
            gameOverPanel.SetActive(true);
            leftWinText.SetActive(true);
        }
        if (gameOverPanel && Input.GetKeyDown(KeyCode.Space))
        {
            RestartGame();
        }
        if (Input.GetKeyDown(KeyCode.Backspace))
        {
            BuildingData[] buildings = new BuildingData[GameManager.Instance.buildingsDictionary.Count];
            int i = 0;
            foreach (KeyValuePair<string, Building> entry in GameManager.Instance.buildingsDictionary)
            {
                buildings[i] = new BuildingData
                {
                    id = entry.Key,
                    type = entry.Value.buildingType.ToString(),
                    side = entry.Value.gameObject.tag,
                    level = 0
                };
                i++;
            }

            GameMessage message = new GameMessage
            {
                action = "startGame",
                data = new GameData
                {
                    attackerId = this.gameObject.name, // or some unique ID for the unit
                    buildings = buildings,
                },
                token = PlayerPrefs.GetString("accessToken")
            };

            string jsonMessage = JsonUtility.ToJson(message);
            WebSocketClient.Instance.SendWebSocketMessage(jsonMessage);
        }
    }

    void RestartGame()
    {
        SceneManager.LoadScene(Application.loadedLevel);
    }
}

[System.Serializable]
public class GameMessage
{
    public string action;
    public GameData data;
    public string token;
}

[System.Serializable]
public class GameData
{
    public string attackerId;
    public BuildingData[] buildings;
}