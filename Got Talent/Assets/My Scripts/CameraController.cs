using System.Diagnostics;
using Cinemachine;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    [SerializeField] private CinemachineVirtualCamera[] _cinemachineVcam;
    
    private void OnEnable()
    {
        EventManager.OnPerformanceStart.AddListener(SwitchToPerformanceCam);
        EventManager.OnPerformanceEnd.AddListener(SwitchToVoteCam);
        EventManager.OnVotingEnd.AddListener(SwitchToFirstCam);
    }

    private void OnDisable()
    {
        EventManager.OnPerformanceStart.RemoveListener(SwitchToPerformanceCam);
        EventManager.OnPerformanceEnd.RemoveListener(SwitchToVoteCam);
        EventManager.OnVotingEnd.RemoveListener(SwitchToFirstCam);
    }

    private void SwitchToPerformanceCam()
    {
        _cinemachineVcam[0].Priority = 0;
        _cinemachineVcam[1].Priority = 1;
        LookAtContestant(1);
    }

    private void SwitchToVoteCam()
    {
        _cinemachineVcam[1].Priority = 0;
        _cinemachineVcam[2].Priority = 1;
    }

    private void SwitchToFirstCam()
    {
        for (int i = 0; i < _cinemachineVcam.Length; i++)
        {
            if(_cinemachineVcam[i] != null)
                _cinemachineVcam[i].LookAt = null;
        }
        
        _cinemachineVcam[0].Priority = 1;
        _cinemachineVcam[2].Priority = 0;
    }

    private void LookAtContestant(int index)
    {
        Contestant contestant = FindObjectOfType<Contestant>();
        _cinemachineVcam[index].LookAt = contestant.transform;
    }
}
