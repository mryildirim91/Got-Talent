using System;
using System.Collections;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class Vote : MonoBehaviour
{
    private int _votingRound = 1;
    private bool _playerCanVote;
    private Vector2 _fingerDownPosition;
    private Vector2 _fingerUpPosition;
    private const float MINDistanceForSwipe = 100;
    private IContestantBeingVoted _beingVoted;
    private AudioSource _audioSource;
    [SerializeField] private AudioClip[] _clips;
    [SerializeField] private GameObject[] _arms;
    [SerializeField] private Image[] _voteImages;
    [SerializeField] private Sprite _yesSprite, _noSprite;

    private void Awake()
    {
        _audioSource = GetComponent<AudioSource>();
    }

    private void OnEnable()
    {
        EventManager.OnPerformanceEnd.AddListener(BeginVoting);
    }
    private void OnDisable()
    {
        EventManager.OnPerformanceEnd.RemoveListener(BeginVoting);
    }
    private void BeginVoting()
    {
        _beingVoted = FindObjectOfType<Contestant>().GetComponent<IContestantBeingVoted>();
        StartCoroutine(VotingRoutine());
    }
    
    private IEnumerator VotingRoutine()
    {
        var juryIndex = 0;
        var randomNum = Random.Range(-1, 2);
        
        if (randomNum == 0)
        {
            randomNum = 1;
        }
        
        yield return BetterWaitForSeconds.Wait(3);
        JuryVote(randomNum,juryIndex);
        randomNum = -randomNum;
        juryIndex++;
        yield return BetterWaitForSeconds.Wait(2);
        JuryVote(randomNum,juryIndex);
        yield return BetterWaitForSeconds.Wait(2);
        _playerCanVote = true;
    }

    private void JuryVote(int randomNum, int juryIndex)
    {
        var animIndex = 1;

        _voteImages[juryIndex].gameObject.SetActive(true);
        
        if (randomNum == -1)
        {
            _audioSource.PlayOneShot(_clips[2]);
            animIndex = 0;
            _voteImages[juryIndex].sprite = _noSprite;
        }
        else
        {
            _audioSource.PlayOneShot(_clips[juryIndex]);
            _voteImages[juryIndex].sprite = _yesSprite;
        }
        
        _beingVoted.VoteAnimation(animIndex);

    }
    private void EndVote()
    {
        foreach (var image in _voteImages)
        {
            image.gameObject.SetActive(false);
        }

        foreach (var arm in _arms)
        {
            if (arm.activeSelf)
            {
                var pos = arm.transform.position;
                pos.y = 4f;
                arm.transform.position = pos;
                
                arm.SetActive(false);
            }
        }
        
        if (_votingRound == 3)
        {
            EventManager.OnLevelComplete.Invoke();
            return;
        }
        EventManager.OnVotingEnd.Invoke();
        
        _votingRound++;
    }
    

    
    private void Update()
    {
        if (!_playerCanVote)
            return;
        
        PlayerVote();
    }

    private void PlayerVote()
    {
        foreach (Touch touch in Input.touches)
        {
            if (touch.phase == TouchPhase.Began)
            {
                _fingerUpPosition = touch.position;
                _fingerDownPosition = touch.position;
            }

            if (touch.phase == TouchPhase.Moved)
            {
                _fingerDownPosition = touch.position;
                DetectSwipe();
            }

            if (touch.phase == TouchPhase.Ended)
            {
                _fingerDownPosition = touch.position;
                DetectSwipe();
            }
        }
    }

    private void DetectSwipe()
    {
        if (!_playerCanVote) return;
        
        if (Mathf.Abs(_fingerDownPosition.x - _fingerUpPosition.x) > MINDistanceForSwipe)
        {
            _voteImages[2].gameObject.SetActive(true);
            
            if (_fingerDownPosition.x - _fingerUpPosition.x > 0)
            {
                _audioSource.PlayOneShot(_clips[1]);
                _arms[0].SetActive(true);
                _arms[0].transform.DOMoveY(3.556f, 0.3f);
                _voteImages[2].sprite = _yesSprite;
                _beingVoted.VoteAnimation(1);
                UITop.Instance.GiveStars();
            }
            else
            {
                _audioSource.PlayOneShot(_clips[2]);
                _arms[1].SetActive(true);
                _arms[1].transform.DOMoveY(3.556f, 0.3f);
                _voteImages[2].sprite = _noSprite;
                _beingVoted.VoteAnimation(0);
            }

            _playerCanVote = false;
            Invoke(nameof(EndVote),2);
            _fingerUpPosition = _fingerDownPosition;
        }
    }
}
