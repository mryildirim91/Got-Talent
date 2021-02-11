using UnityEngine;

public class ContestantEndState : StateMachineBehaviour
{
    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        EventManager.OnPerformanceEnd.Invoke();
    }
}
