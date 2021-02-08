using UnityEngine;

public class StartState : StateMachineBehaviour
{
    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        EventManager.OnPerformanceStart.Invoke();
    }
}
