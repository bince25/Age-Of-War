using UnityEngine;

public class UIManager : MonoBehaviour
{
    public GameObject loginPanel;
    public GameObject mainMenuPanel;
    public GameObject enterGamePanel;
    public GameObject gameModePanel;
    public GameObject createGamePanel;
    public GameObject joinGamePanel;
    public GameObject lobbyPanel;

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

        // Show the requested panel
        panel.SetActive(true);
    }

}