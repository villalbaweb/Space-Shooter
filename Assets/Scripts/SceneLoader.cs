using System;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLoader : MonoBehaviour
{
    [Header("Game Over Control")]
    [SerializeField] float timeToGameOverLoad = 3.0f;

    GameSession _gameSession;

    private void Start()
    {
        _gameSession = FindObjectOfType<GameSession>();
    }

    public void LoadGameOver()
    {
        StartCoroutine(LoadGameOverLevel());
    }

    private IEnumerator LoadGameOverLevel()
    {
        yield return new WaitForSeconds(timeToGameOverLoad);
        SceneManager.LoadScene("Game Over");
    }

    public void QuitGame()
    {
        Application.Quit();
    }

    public void LoadMainMenu()
    {
        SceneManager.LoadScene("Start Menu");
    }

    public void LoadPlayLevel()
    {
        if(_gameSession != null)
        {
            _gameSession.RestGameSession();
        }
        SceneManager.LoadScene("Game");
    }
}
