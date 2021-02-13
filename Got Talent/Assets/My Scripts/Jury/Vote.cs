using System.Collections;
using UnityEngine;

public class Vote : MonoBehaviour
{
    private int _votingRound = 1;
    private bool _playerCanVote;
    private IContestantBeingVoted _beingVoted;
    private Animator[] _animators;
    [SerializeField] private GameObject[] _juries;
    [SerializeField] private Sprite[] _popSprites;

    private void Awake()
    {
        _animators = new Animator[_juries.Length];
        for (int i = 0; i < _animators.Length; i++)
        {
            _animators[i] = _juries[i].GetComponent<Animator>();
        }
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
        yield return BetterWaitForSeconds.Wait(3);
        _beingVoted.VoteAnimation(1);
        _animators[0].SetTrigger("Approve");
        _juries[0].transform.GetChild(0).gameObject.SetActive(true);
        yield return BetterWaitForSeconds.Wait(2);
        _beingVoted.VoteAnimation(0);
        _animators[1].SetTrigger("Disapprove");
        _juries[1].transform.GetChild(0).gameObject.SetActive(true);
        yield return BetterWaitForSeconds.Wait(2);
        _playerCanVote = true;
        StopCoroutine(nameof(VotingRoutine));
    }

    public void PlayerVote(bool aprroved)
    {
        if (_playerCanVote)
        {
            _juries[2].transform.GetChild(0).gameObject.SetActive(true);
            
            if (aprroved)
            {
                _juries[2].transform.GetChild(0).GetComponent<SpriteRenderer>().sprite = _popSprites[0];
                _animators[2].SetTrigger("Approve");
                _beingVoted.VoteAnimation(1);
                UITop.Instance.GiveStars();
            }
            else
            {
                _juries[2].transform.GetChild(0).GetComponent<SpriteRenderer>().sprite = _popSprites[1];
                _animators[2].SetTrigger("Disapprove");
                _beingVoted.VoteAnimation(0);
            }

            _playerCanVote = false;
            Invoke(nameof(EndVote),2);
        }

    }
    private void EndVote()
    {
        for (int i = 0; i < _juries.Length; i++)
        {
            _juries[i].transform.GetChild(0).gameObject.SetActive(false);
        }
        if (_votingRound == 2)
        {
            EventManager.OnLevelComplete.Invoke();
            return;
        }
        EventManager.OnVotingEnd.Invoke();
        
        _votingRound++;
    }
}
