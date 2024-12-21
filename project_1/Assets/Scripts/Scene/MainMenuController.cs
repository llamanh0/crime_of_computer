using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuController : MonoBehaviour
{
    [SerializeField] private string mainGame;

    public void StartGame()
    {
        StartCoroutine(StartGameWithDelay());
    }

    private IEnumerator StartGameWithDelay()
    {
        yield return new WaitForSeconds(1f); // 1 saniye bekler
        SceneManager.LoadScene(mainGame);
    }

    public void QuitGame()
    {
        Application.Quit();
    }
}