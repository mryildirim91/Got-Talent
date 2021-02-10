using System;
using System.Collections;
using UnityEngine;

public class ContestantAnimation : MonoBehaviour, IContestantBeingVoted
{
    private Animator _animator;
    [SerializeField] private string _animation;

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
        _animator.SetTrigger("Idle");
    }

    private void WalkAnimation()
    {
        _animator.SetTrigger("Walk");
    }

    private void PerformanceAnimation()
    {
        _animator.SetTrigger(_animation);
    }

    private IEnumerator VoteAnimationDelay(int num)
    {
        yield return BetterWaitForSeconds.Wait(1);
        
        if (num == 1)
        {
            _animator.SetTrigger("Got Yes");
        }

        if (num == 0)
        {
            _animator.SetTrigger("Got No");
        }
        
        StopCoroutine(VoteAnimationDelay(num));
    }
    
    public void VoteAnimation(int num)
    {
        StartCoroutine(VoteAnimationDelay(num));
    }
}
