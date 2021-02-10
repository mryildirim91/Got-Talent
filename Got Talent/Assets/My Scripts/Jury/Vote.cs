using System.Collections;
using UnityEngine;

public class Vote : MonoBehaviour
{
    private int _currentJury;
    private int _approval, _disapproval;
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
        PlayerVote();
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
        PlayerCanVote();
        StopCoroutine(nameof(VotingRoutine));
    }
    private void PlayerCanVote()
    {
        EventManager.OnPlayerCanVote.Invoke();

        _playerCanVote = true;
    }

    private void PlayerVote()
    {
        if (_playerCanVote)
        {
            if (Input.GetKeyDown(KeyCode.Y))
            {
                _beingVoted.VoteAnimation(1);
                _animators[2].SetTrigger("Approve");
                _approval++;
            }

            if (Input.GetKeyDown(KeyCode.N))
            {
                _beingVoted.VoteAnimation(0);
                _animators[2].SetTrigger("Disapprove");
                _disapproval++;
            }

            if (Input.anyKeyDown)
            {
                _playerCanVote = false;
                Invoke(nameof(EndVote),2);
            }
        }
    }

    private void EndVote()
    {
        EventManager.OnVotingEnd.Invoke();
    }
}
