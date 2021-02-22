using UnityEngine;
using UnityEngine.SceneManagement;
using LionStudios;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }
    public bool IsGameStarted { get; private set; }

    private void Awake()
    {
        Instance = this;
        Analytics.Events.LevelStarted(PlayerPrefs.GetInt("CurrentEpisode") + 1);
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
        Analytics.Events.LevelComplete(PlayerPrefs.GetInt("CurrentEpisode") + 1);
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
