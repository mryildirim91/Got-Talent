using System;
using UnityEngine;
using UnityEngine.UI;

public class ContestantSpawner : MonoBehaviour
{
    private GameObject _clone;
    [SerializeField] private Transform _spawnPos;
    [SerializeField] private GameObject[] _contestants;
    [SerializeField] private Text[] _nameText, _talentText;
    [SerializeField] private Image[] _contestantPicture;
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
        
        if (!GameManager.Instance.IsGameStarted)
        {
            ContestantAnimation[] _contestant = new ContestantAnimation[_nameText.Length];
            
            for (int i = 0; i < _contestant.Length; i++)
            {
                if (PlayerPrefs.GetInt("ContestantIndex") + i > _contestants.Length - 1)
                {
                    _contestant[i] = _contestants[0 + i].GetComponent<ContestantAnimation>();
                }
                else
                {
                    _contestant[i] = _contestants[PlayerPrefs.GetInt("ContestantIndex") + i].GetComponent<ContestantAnimation>(); 
                }
                _nameText[i].text = _contestant[i]._contestantInfo.Name;
                _talentText[i].text = _contestant[i]._contestantInfo.Talent;
                _contestantPicture[i].sprite = _contestant[i]._contestantInfo.Picture;
            }
        }
        PlayerPrefs.SetInt("ContestantIndex",PlayerPrefs.GetInt("ContestantIndex") + 1);
    }
}
