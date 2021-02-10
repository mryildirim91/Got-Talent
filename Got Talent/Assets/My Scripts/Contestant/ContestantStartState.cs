using UnityEngine;

public class ContestantStartState : StateMachineBehaviour
{
    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        EventManager.OnPerformanceStart.Invoke();
    }
}
