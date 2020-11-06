using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class W5_ZombieAttackState : StateMachineBehaviour
{
    public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        base.OnStateExit(animator, stateInfo, layerIndex);

        W5_AIMovement aiMovement = animator.GetComponent<W5_AIMovement>();

        if(aiMovement)
        {
            aiMovement.ChangeState(W5_AIMovement.EAIState.Idle);
        }
    }
}
