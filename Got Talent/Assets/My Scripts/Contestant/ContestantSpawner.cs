using System;
using UnityEngine;
public class ContestantSpawner : MonoBehaviour
{
    private GameObject _clone;
    [SerializeField] private Transform _spawnPos;
    [SerializeField] private GameObject[] _contestants;
    private void Start()
    {
        SpawnContestant();
    }

    private void OnEnable()
    {
        EventManager.OnPlayerLeftStage.AddListener(SpawnContestant);
    }

    private void OnDisable()
    {
        EventManager.OnPlayerLeftStage.RemoveListener(SpawnContestant);
    }

    private void SpawnContestant()
    {
        if (PlayerPrefs.GetInt("ContestantIndex") >= _contestants.Length)
        {
            PlayerPrefs.DeleteKey("ContestantIndex");
        }
        _clone = Instantiate(_contestants[PlayerPrefs.GetInt("ContestantIndex")]);
        _clone.transform.position = _spawnPos.position;
        PlayerPrefs.SetInt("ContestantIndex",PlayerPrefs.GetInt("ContestantIndex") + 1);
    }
}
