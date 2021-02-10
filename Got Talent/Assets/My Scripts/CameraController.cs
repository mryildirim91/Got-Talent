using Cinemachine;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    [SerializeField] private CinemachineVirtualCamera[] _cinemachineVcam;
    
    private void OnEnable()
    {
        EventManager.OnPerformanceStart.AddListener(SwitchToPerformanceCam);
        EventManager.OnPerformanceEnd.AddListener(SwitchToVoteCam);
        EventManager.OnVotingEnd.AddListener(StopFollowingContestant);
    }

    private void OnDisable()
    {
        EventManager.OnPerformanceStart.RemoveListener(SwitchToPerformanceCam);
        EventManager.OnPerformanceEnd.RemoveListener(SwitchToVoteCam);
        EventManager.OnVotingEnd.RemoveListener(StopFollowingContestant);
    }

    private void SwitchToPerformanceCam()
    {
        Contestant contestant = FindObjectOfType<Contestant>();
        _cinemachineVcam[0].Priority = 0;
        _cinemachineVcam[1].Priority = 1;
        _cinemachineVcam[1].LookAt = contestant.transform;
    }

    private void SwitchToVoteCam()
    {
        Contestant contestant = FindObjectOfType<Contestant>();
        _cinemachineVcam[1].Priority = 0;
        _cinemachineVcam[2].Priority = 1;
        _cinemachineVcam[2].LookAt = contestant.transform;
    }

    private void StopFollowingContestant()
    {
        for (int i = 0; i < _cinemachineVcam.Length; i++)
        {
            _cinemachineVcam[i].LookAt = null;
        }
        
        _cinemachineVcam[0].Priority = 1;
        _cinemachineVcam[2].Priority = 0;
    }
}
