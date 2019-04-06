using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "StateMachine/Actions/Chase")]
public class ChaseAction : Action
{
    public override void Act(AIController controller)
    {
        ChaseTarget(controller);
    }

   private void ChaseTarget(AIController controller)
    {
        if (controller.chasingTarget == null)
            return;

        Vector3 targetPos = controller.chasingTarget.position;
        targetPos.y = controller.transform.position.y;

        controller.MoveTo(targetPos);
    }
}
