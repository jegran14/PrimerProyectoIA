using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Acción de patrullar
[CreateAssetMenu(menuName = "StateMachine/Actions/Patrol")]
public class PatrolAction : Action
{
    public override void Act(AIController controller)
    {
        Patrol(controller);
    }

    /// <summary>
    /// Ordenar al personaje de patrullar
    /// </summary>
    /// <param name="controller">Referencia al controlador de la IA</param>
    private void Patrol(AIController controller)
    {
        Vector3 currentWaypoint = controller.GetWayPoint(controller.currentWayPoint).position;
        currentWaypoint.y = controller.transform.position.y;

        if (controller.IsAtTargetPos())
        {
            controller.currentWayPoint++;
            if (controller.currentWayPoint >= controller.wayPoints.Length)
                controller.currentWayPoint = 0;

            currentWaypoint = controller.GetWayPoint(controller.currentWayPoint).position;
        }

        controller.MoveTo(currentWaypoint);
    }
}
