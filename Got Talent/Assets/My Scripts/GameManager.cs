using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }
    public bool IsGameStarted { get; private set; }

    private void Awake()
    {
        Instance = this;
    }

    private void OnEnable()
    {
        EventManager.OnLevelComplete.AddListener(LevelComplete);
    }

    private void OnDisable()
    {
        EventManager.OnLevelComplete.RemoveListener(LevelComplete);
    }
    
    private void LevelComplete()
    {
        PlayerPrefs.SetInt("CurrentEpisode", PlayerPrefs.GetInt("CurrentEpisode") + 1);
    }

    public void GoNextLevel()
    {
        SceneManager.LoadSceneAsync(0);
    }

    public void StartGame()
    {
        IsGameStarted = true;
    }
}
