using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "StateMachine/Actions/Patrol")]
public class PatrolAction : Action
{
    public override void Act(AIController controller)
    {
        throw new System.NotImplementedException();
    }

    private void Patrol(AIController controller)
    {
        controller.MoveTo(controller.wayPointList[controller.nextWayPoint].position);
    }
}
