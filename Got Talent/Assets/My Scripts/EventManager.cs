using UnityEngine.Events;

public static class EventManager
{
    public static UnityEvent OnReachDestination = new UnityEvent();
    public static UnityEvent OnPerformanceStart = new UnityEvent();
    public static UnityEvent OnPerformanceEnd = new UnityEvent();
    public static UnityEvent OnVotingEnd = new UnityEvent();
    public static UnityEvent OnPlayerLeftStage = new UnityEvent();
    public static UnityEvent OnLevelComplete = new UnityEvent();
}
