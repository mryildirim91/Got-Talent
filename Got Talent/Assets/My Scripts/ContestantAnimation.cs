using System;
using UnityEngine;

public class ContestantAnimation : MonoBehaviour
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
    }

    private void OnDisable()
    {
        EventManager.OnReachDestination.RemoveListener(IdleAnimation);
        EventManager.OnPerformanceStart.RemoveListener(PerformanceAnimation);
        EventManager.OnPerformanceEnd.RemoveListener(WalkAnimation);
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

    private void GotYesAnimation()
    {
        _animator.SetTrigger("Got Yes");
    }

    private void GotNoAnimation()
    {
        _animator.SetTrigger("Got No");
    }
}
