using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EndState : StateMachineBehaviour
{
    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        EventManager.OnPerformanceEnd.Invoke();
    }
}
