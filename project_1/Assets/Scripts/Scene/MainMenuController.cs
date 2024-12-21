using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuController : MonoBehaviour
{
    [Header("Scene Management")]
    [SerializeField] private string mainGameSceneName = "MainGame";
    [SerializeField] private float startDelay = 1f; // Sahne geçişinden önce bekleme süresi

    public void StartGame()
    {
        StartCoroutine(StartGameWithDelay());
    }

    private IEnumerator StartGameWithDelay()
    {
        yield return new WaitForSeconds(startDelay);
        SceneManager.LoadScene(mainGameSceneName);
    }

    public void QuitGame()
    {
        Application.Quit();
    }
}