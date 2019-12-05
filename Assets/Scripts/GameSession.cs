using UnityEngine;

public class GameSession : MonoBehaviour
{
    // State
    int _score = 0;

    private void Awake()
    {
        #region Singleton Pattern

        int gameSessionCount = FindObjectsOfType<GameSession>().Length;
        if(gameSessionCount > 1)
        {
            gameObject.SetActive(false);
            Destroy(gameObject);
        }
        else
        {
            DontDestroyOnLoad(gameObject);
        }

        #endregion
    }

    public void IncreaseScore(int increaseScore)
    {
        _score += increaseScore;
    }

    public int GetCurrentScore()
    {
        return _score;
    }

    public void RestGameSession()
    {
        Destroy(gameObject);
    }
}
