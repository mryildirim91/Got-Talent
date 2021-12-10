using System;
using UnityEngine;
using UnityEngine.UI;

public class ContestantSpawner : MonoBehaviour
{
    private GameObject _contestantClone;
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
        _contestantClone = Instantiate(_contestants[PlayerPrefs.GetInt("ContestantIndex")]);
        _contestantClone.transform.position = _spawnPos.position;
        
        if (!GameManager.Instance.IsGameStarted)
        {
            var contestant = new ContestantAnimation[_nameText.Length];
            
            for (int i = 0; i < contestant.Length; i++)
            {
                if (PlayerPrefs.GetInt("ContestantIndex") + i > _contestants.Length - 1)
                {
                    contestant[i] = _contestants[0 + i].GetComponent<ContestantAnimation>();
                }
                else
                {
                    contestant[i] = _contestants[PlayerPrefs.GetInt("ContestantIndex") + i].GetComponent<ContestantAnimation>(); 
                }
                _nameText[i].text = contestant[i].Info.Name;
                _talentText[i].text = contestant[i].Info.Talent;
                _contestantPicture[i].sprite = contestant[i].Info.Picture;
            }
        }
        PlayerPrefs.SetInt("ContestantIndex",PlayerPrefs.GetInt("ContestantIndex") + 1);
    }
}
