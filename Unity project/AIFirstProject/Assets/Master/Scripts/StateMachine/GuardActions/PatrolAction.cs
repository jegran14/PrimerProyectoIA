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
        if(controller.wayPoints == null || controller.currentWaypointIndex < controller.wayPoints.Length)
        {
            Vector3 currentWaypoint = controller.wayPoints[controller.currentWaypointIndex].position;

            if (controller.IsAtTargetPos())
            {
                controller.currentWaypointIndex++;
                if (controller.currentWaypointIndex >= controller.wayPoints.Length)
                    controller.currentWaypointIndex = 0;

                currentWaypoint = controller.wayPoints[controller.currentWaypointIndex].position;
            }

            currentWaypoint.y = controller.transform.position.y;

            controller.MoveTo(currentWaypoint);
        }
    }
}
