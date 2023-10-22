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
    private Ages leftAge, rightAge;

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
            GameMessage message = new GameMessage
            {
                action = "startGame",
                data = new GameData
                {
                    attackerId = this.gameObject.name, // or some unique ID for the unit
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
}