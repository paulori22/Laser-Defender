using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Level : MonoBehaviour
{
    public void LoadStartMenu()
    {
        SceneManager.LoadScene(0);
    }
    public void LoadGame()
    {
        SceneManager.LoadScene("Game");
        FindObjectOfType<GameSession>().ResetGame();
    }

    public void LoadGameOver(float delayTimeInSeconds = 2f)
    {
        StartCoroutine(DelayAndLoadGameOver(delayTimeInSeconds));
    }

    IEnumerator DelayAndLoadGameOver(float delayTimeInSeconds)
    {
        yield return new WaitForSeconds(delayTimeInSeconds);
        SceneManager.LoadScene("Game Over");
    }
    public void QuitGame()
    {
        Application.Quit();
    }
}
