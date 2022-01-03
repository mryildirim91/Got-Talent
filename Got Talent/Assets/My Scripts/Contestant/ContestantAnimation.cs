using System;
using System.Collections;
using UnityEngine;

public class ContestantAnimation : MonoBehaviour, IContestantBeingVoted
{
    private Animator _animator;

    [SerializeField]private ContestantInfo _contestantInfo;
    public ContestantInfo Info => _contestantInfo;
    private static readonly int Idle = Animator.StringToHash("Idle");
    private static readonly int Walk = Animator.StringToHash("Walk");
    private static readonly int Yes = Animator.StringToHash("Got Yes");
    private static readonly int No = Animator.StringToHash("Got No");

    [Serializable]
    public struct ContestantInfo
    {
        [SerializeField]private string _name;
        [SerializeField]private string _talent;
        [SerializeField]private Sprite _picture;

        public string Name => _name;
        public string Talent => _talent;
        public Sprite Picture => _picture;
    }

    private void Start()
    {
        _animator = GetComponent<Animator>();
    }
    private void OnEnable()
    {
        EventManager.OnReachDestination.AddListener(IdleAnimation);
        EventManager.OnPerformanceStart.AddListener(PerformanceAnimation);
        EventManager.OnPerformanceEnd.AddListener(WalkAnimation);
        EventManager.OnVotingEnd.AddListener(WalkAnimation);
    }
    private void OnDisable()
    {
        EventManager.OnReachDestination.RemoveListener(IdleAnimation);
        EventManager.OnPerformanceStart.RemoveListener(PerformanceAnimation);
        EventManager.OnPerformanceEnd.RemoveListener(WalkAnimation);
        EventManager.OnVotingEnd.RemoveListener(WalkAnimation);
    }
    private void IdleAnimation()
    {
        _animator.SetTrigger(Idle);
    }
    private void WalkAnimation()
    {
        _animator.SetTrigger(Walk);
    }
    private void PerformanceAnimation()
    {
        _animator.SetTrigger(_contestantInfo.Talent);
    }
    private IEnumerator VoteAnimationDelay(int num)
    {
        yield return BetterWaitForSeconds.Wait(0.25f);
        
        switch (num)
        {
            case 1:
                _animator.SetTrigger(Yes);
                break;
            case 0:
                _animator.SetTrigger(No);
                break;
        }
    }
    public void VoteAnimation(int num)
    {
        StartCoroutine(VoteAnimationDelay(num));
    }
}
