using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LoadingScreen : MonoBehaviour
{
    public GameObject loadingPanel;
    public Slider progressBar;
    public TMP_Text loadingText;

    private void Awake()
    {
        HideLoadingPanel();
        progressBar.gameObject.SetActive(false);
    }

    // Call this method to load a scene
    public void LoadScene(string sceneName)
    {
        progressBar.gameObject.SetActive(true);
        loadingPanel.SetActive(true);
        StartCoroutine(BeginLoad(sceneName));
    }

    public void LoadScene(int sceneName)
    {
        progressBar.gameObject.SetActive(true);
        loadingPanel.SetActive(true);
        StartCoroutine(BeginLoad(sceneName));
    }

    private IEnumerator BeginLoad(string sceneName)
    {
        AsyncOperation operation = SceneManager.LoadSceneAsync(sceneName);

        while (!operation.isDone)
        {
            // Unity's AsyncOperation progress goes from 0 to 0.9 when it's ready to switch, the last 0.1 is when the switch happens.
            float progress = Mathf.Clamp01(operation.progress / 0.9f);
            progressBar.value = progress;
            loadingText.text = "Loading... " + (int)(progress * 100) + "%";

            yield return null;
        }
        progressBar.gameObject.SetActive(false);
    }

    private IEnumerator BeginLoad(int sceneName)
    {
        AsyncOperation operation = SceneManager.LoadSceneAsync(sceneName);

        while (!operation.isDone)
        {
            // Unity's AsyncOperation progress goes from 0 to 0.9 when it's ready to switch, the last 0.1 is when the switch happens.
            float progress = Mathf.Clamp01(operation.progress / 0.9f);
            progressBar.value = progress;
            loadingText.text = "Loading... " + (int)(progress * 100) + "%";

            yield return null;
        }
        progressBar.gameObject.SetActive(false);
    }

    public void ShowLoadingPanel(string message = "Loading...")
    {
        loadingPanel.SetActive(true);
        loadingText.text = message;
    }

    public void HideLoadingPanel()
    {
        loadingPanel.SetActive(false);
    }

    public void UpdateProgressBar(float progress)
    {
        progressBar.value = progress;
    }
}
