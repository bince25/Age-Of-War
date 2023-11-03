using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ModalController : MonoBehaviour
{
    public GameObject modalPanel;
    public GameObject modalPanelBackground;
    public GameObject messagePanel;
    public TMP_Text titleText;
    public TMP_Text messageText;
    public Button okButton;
    public Button cancelButton;

    void Start()
    {
        // Initially hide the modal
        modalPanel.SetActive(false);

        // Configure button listeners
        okButton.onClick.AddListener(OnOkClicked);
        // cancelButton.onClick.AddListener(OnCancelClicked);
    }

    public void ShowModal(string title, string message)
    {
        titleText.text = title;
        messageText.text = message;
        modalPanel.SetActive(true);
    }

    public void ShowModal(string title, string message, Color color)
    {
        titleText.text = title;
        messageText.text = message;
        messagePanel.GetComponent<Image>().color = color;
        modalPanel.SetActive(true);
    }

    public void ShowModal(string title, string message, Color color, Color okButtonColor)
    {
        titleText.text = title;
        messageText.text = message;
        messagePanel.GetComponent<Image>().color = color;
        okButton.GetComponent<Image>().color = okButtonColor;
        modalPanel.SetActive(true);
    }


    private void OnOkClicked()
    {
        // Handle OK click
        modalPanel.SetActive(false);
        // Optionally re-enable other UI elements or resume the game
    }

    private void OnCancelClicked()
    {
        // Handle Cancel click
        modalPanel.SetActive(false);
        // Optionally re-enable other UI elements or resume the game
    }
}
