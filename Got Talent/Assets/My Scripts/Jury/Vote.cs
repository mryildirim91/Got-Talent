using System.Collections;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class Vote : MonoBehaviour
{
    private int _votingRound = 1;
    private bool _playerCanVote;
    private IContestantBeingVoted _beingVoted;
    [SerializeField] private GameObject[] _arms;
    [SerializeField] private Image[] _voteImages;
    [SerializeField] private Sprite _yesSprite, _noSprite;
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
        int juryIndex = 0;
        int randomNum = Random.Range(-1, 2);
        
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
        int animIndex = 1;

        _voteImages[juryIndex].gameObject.SetActive(true);
        
        if (randomNum == -1)
        {
            animIndex = 0;
            _voteImages[juryIndex].sprite = _noSprite;
        }
        else
        {
            _voteImages[juryIndex].sprite = _yesSprite;
        }
        
        _beingVoted.VoteAnimation(animIndex);

    }
    private void EndVote()
    {
        for (int i = 0; i < _voteImages.Length; i++)
        {
            _voteImages[i].gameObject.SetActive(false);
        }

        for (int i = 0; i < _arms.Length; i++)
        {
            if (_arms[i].activeSelf)
            {
                var pos = _arms[i].transform.position;
                pos.y = 4f;
                _arms[i].transform.position = pos;
                
                _arms[i].SetActive(false);
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
    
    private Vector2 _fingerDownPosition;
    private Vector2 _fingerUpPosition;
    
    private bool _detectSwipeOnlyAfterRelease;
    
    private float _minDistanceForSwipe = 100;
    
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

            if (!_detectSwipeOnlyAfterRelease && touch.phase == TouchPhase.Moved)
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
        if (_playerCanVote)
        {
            if (Mathf.Abs(_fingerDownPosition.x - _fingerUpPosition.x) > _minDistanceForSwipe)
            {
                _voteImages[2].gameObject.SetActive(true);
            
                if (_fingerDownPosition.x - _fingerUpPosition.x > 0)
                {
                    _arms[0].SetActive(true);
                    _arms[0].transform.DOMoveY(3.556f, 0.3f);
                    _voteImages[2].sprite = _yesSprite;
                    _beingVoted.VoteAnimation(1);
                    UITop.Instance.GiveStars();
                }
                else
                {
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
}
