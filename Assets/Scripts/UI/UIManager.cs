using UnityEngine;

public class UIManager : MonoBehaviour
{
    // Singleton pattern
    public static UIManager Instance { get; private set; }

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
            // Destroy the duplicate
            Destroy(gameObject);
        }
    }

    public GameObject loginPanel;
    public GameObject mainMenuPanel;
    public GameObject enterGamePanel;
    public GameObject gameModePanel;
    public GameObject createGamePanel;
    public GameObject joinGamePanel;
    public GameObject lobbyPanel;
    public GameObject countryPanel;

    // Call this method to transition between panels
    public void ShowPanel(GameObject panel)
    {
        // Hide all panels
        loginPanel.SetActive(false);
        mainMenuPanel.SetActive(false);
        joinGamePanel.SetActive(false);
        lobbyPanel.SetActive(false);
        gameModePanel.SetActive(false);
        createGamePanel.SetActive(false);
        countryPanel.SetActive(false);

        // Show the requested panel
        panel.SetActive(true);
    }

    public void ClosePanel(GameObject panel)
    {
        panel.SetActive(false);
    }
}