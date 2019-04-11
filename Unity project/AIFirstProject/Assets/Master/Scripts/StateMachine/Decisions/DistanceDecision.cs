using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "StateMachine/Decisions/DistanceDesicion")]
public class DistanceDecision : Decision
{
    [Tooltip("Tipo de comparacion que se debe hacer con la distancia")]
    public ComparisonSigns theDistanceIs;

    public override bool Decide(AIController controller)
    {
        bool decision = CalculateDistance(controller);
        return decision;
    }

    /// <summary>
    /// Calcular la distancia desde el personaje hasta el punto de medio de la patrulla
    /// </summary>
    /// <param name="controller"></param>
    /// <returns></returns>
    private bool CalculateDistance(AIController controller)
    {
        float distance = Vector3.Distance(controller.chasingTarget.position, controller.patrolMiddlePoint);

        bool isInDistance = false;

        switch(theDistanceIs)
        {
            case ComparisonSigns.Greater:
                isInDistance = distance > controller.chasingMaxDistance;
                break;
            case ComparisonSigns.Equal:
                isInDistance = distance == controller.chasingMaxDistance;
                break;
            case ComparisonSigns.Lower:
                isInDistance = distance < controller.chasingMaxDistance;
                break;
        }


        return isInDistance;
    }
}
