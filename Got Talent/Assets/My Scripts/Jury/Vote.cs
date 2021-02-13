using System.Collections;
using DG.Tweening;
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
        int juryIndex = 0;
        int randomNum = Random.Range(-1, 2);
        
        if (randomNum == 0)
        {
            randomNum = 1;
        }
        
        yield return BetterWaitForSeconds.Wait(3);
        JuryVote(randomNum,juryIndex);
        juryIndex++;
        randomNum = -randomNum;
        yield return BetterWaitForSeconds.Wait(2);
        JuryVote(randomNum,juryIndex);
        yield return BetterWaitForSeconds.Wait(2);
        _playerCanVote = true;
        StopCoroutine(nameof(VotingRoutine));
    }

    private void JuryVote(int randomNum, int juryIndex)
    {
        int animIndex = 1;
        int spriteIndex = 0;
        string juryAnimation = "Approve";
        
        if (randomNum == -1)
        {
            juryAnimation = "Disapprove";
            animIndex = 0;
            spriteIndex = 1;
        }
        
        _beingVoted.VoteAnimation(animIndex);
        _animators[juryIndex].SetTrigger(juryAnimation);
        _juries[juryIndex].transform.GetChild(0).gameObject.SetActive(true);
        _juries[juryIndex].transform.GetChild(0).DOScale(Vector3.one * 0.3f, 0.1f);
        _juries[juryIndex].transform.GetChild(0).GetComponent<SpriteRenderer>().sprite = _popSprites[spriteIndex];
    }
    public void PlayerVote(bool aprroved)
    {
        if (_playerCanVote)
        {
            _juries[2].transform.GetChild(0).gameObject.SetActive(true);
            _juries[2].transform.GetChild(0).DOScale(Vector3.one * 0.3f, 0.1f);
            
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
