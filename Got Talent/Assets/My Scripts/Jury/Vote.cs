using System.Collections;
using UnityEngine;

public class Vote : MonoBehaviour
{
    private int _approval, _disapproval;
    private int _votingRound = 1;
    private bool _playerCanVote;
    private IContestantBeingVoted _beingVoted;
    private Animator[] _animators;
    [SerializeField] private GameObject[] _juries;

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

    private void Update()
    {
        //PlayerVote();
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
        _approval++;
        yield return BetterWaitForSeconds.Wait(3);
        _beingVoted.VoteAnimation(0);
        _animators[1].SetTrigger("Disapprove");
        _disapproval++;
        yield return BetterWaitForSeconds.Wait(3);
        _playerCanVote = true;
        StopCoroutine(nameof(VotingRoutine));
    }
    private void PlayerVote()
    {
        if (_playerCanVote)
        {
            if (Input.GetKeyDown(KeyCode.RightArrow))
            {
                _animators[2].SetTrigger("Approve");
                _beingVoted.VoteAnimation(1);
                _approval++;
            }

            if (Input.GetKeyDown(KeyCode.LeftArrow))
            {
                _animators[2].SetTrigger("Disapprove");
                _beingVoted.VoteAnimation(0);
                _disapproval++;
            }

            if (Input.anyKeyDown)
            {
                _playerCanVote = false;
                Invoke(nameof(EndVote),2);
            }
        }
    }

    public void VoteYes()
    {
        _animators[2].SetTrigger("Approve");
        _beingVoted.VoteAnimation(1);
        _approval++;
        _playerCanVote = false;
        Invoke(nameof(EndVote),2);
    }

    public void VoteNo()
    {
        _animators[2].SetTrigger("Disapprove");
        _beingVoted.VoteAnimation(0);
        _disapproval++;
        _playerCanVote = false;
        Invoke(nameof(EndVote),2);
    }

    private void EndVote()
    {
        if (_votingRound == 2)
        {
            EventManager.OnLevelComplete.Invoke();
            return;
        }
        EventManager.OnVotingEnd.Invoke();
        
        _votingRound++;
    }
}
