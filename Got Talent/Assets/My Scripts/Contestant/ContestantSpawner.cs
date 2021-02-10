using System;
using UnityEngine;
public class ContestantSpawner : MonoBehaviour
{
    private GameObject _clone;
    private int _contestantIndex;
    [SerializeField] private Transform _spawnPos;
    [SerializeField] private GameObject[] _contestants;
    private void Start()
    {
        PlayerPrefs.DeleteAll();
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
        _contestantIndex = PlayerPrefs.GetInt("ContestantIndex");
        _clone = Instantiate(_contestants[_contestantIndex]);
        _clone.transform.position = _spawnPos.position;

        if (_contestantIndex == _contestants.Length - 1)
        {
            _contestantIndex = 0;
        }
        _contestantIndex++;
        PlayerPrefs.SetInt("ContestantIndex",_contestantIndex);
    }
}
