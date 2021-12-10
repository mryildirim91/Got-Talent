using Cinemachine;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    [SerializeField] private CinemachineVirtualCamera[] _cinemachineVcam;
    
    private void OnEnable()
    {
        EventManager.OnPerformanceStart.AddListener(SwitchToPerformanceCam);
        EventManager.OnVotingEnd.AddListener(SwitchToIntermittentCam);
    }

    private void OnDisable()
    {
        EventManager.OnPerformanceStart.RemoveListener(SwitchToPerformanceCam);
        EventManager.OnVotingEnd.RemoveListener(SwitchToIntermittentCam);
    }

    private void SwitchToPerformanceCam()
    {
        LookAtContestant();
        _cinemachineVcam[0].Priority = 0;
        _cinemachineVcam[1].Priority = 0;
        _cinemachineVcam[2].Priority = 1;
    }

    public void SwitchToIntermittentCam()
    {
        _cinemachineVcam[0].Priority = 0;
        _cinemachineVcam[1].Priority = 1;
        _cinemachineVcam[2].Priority = 0;
        
        StopLookingAtContestant();
    }
    private void StopLookingAtContestant()
    {
        foreach (var virtualCamera in _cinemachineVcam)
        {
            if(virtualCamera != null)
                virtualCamera.LookAt = null;
        }
    }

    private void LookAtContestant()
    {
        var contestant = FindObjectOfType<Contestant>();
        _cinemachineVcam[2].LookAt = contestant.transform;
    }
}
