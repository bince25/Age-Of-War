using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;

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
    }

    void RestartGame()
    {
        Application.LoadLevel(Application.loadedLevel);
    }
}
