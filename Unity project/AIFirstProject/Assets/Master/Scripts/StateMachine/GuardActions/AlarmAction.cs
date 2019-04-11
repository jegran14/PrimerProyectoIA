using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu (menuName = "StateMachine/Actions/GiveAlarm")]
public class AlarmAction : Action
{
    [Tooltip("Mascara de los agentes a los que se le vaya a dar la alarma")]
    public LayerMask alarmedAgentsMask;

    public override void Act(AIController controller)
    {
        GiveAlarm(controller);
    }
    
    private void GiveAlarm(AIController controller)
    {
        //Buscar todos los agentes dentro del area de alarma
        Collider[] agents = Physics.OverlapSphere(controller.transform.position, controller.alarmAreaRadius, alarmedAgentsMask);
        //Dar el alarma a cada uno de los agentes
        foreach (Collider agent in agents)
        {
            agent.GetComponent<AIController>().SetAlarm(controller.chasingTarget);
        }
    }
}
