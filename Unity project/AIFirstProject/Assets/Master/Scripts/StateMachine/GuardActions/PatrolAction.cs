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
       // controller.MoveTo(controller.wayPointList[controller.nextWayPoint].position);
    }
}
