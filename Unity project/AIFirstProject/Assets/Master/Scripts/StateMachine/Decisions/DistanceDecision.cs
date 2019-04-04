using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "StateMachine/Decisions/DistanceDesicion")]
public class DistanceDecision : Decision
{
    [Tooltip("Tipo de comparacion que se debe hacer con la distancia")]
    public Comparison theDistanceIs;

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
        Vector3 characterPos = controller.transform.position;
        float distance = Vector3.Distance(characterPos, controller.PatrolMiddlePoint);

        bool isInDistance = false;

        switch(theDistanceIs)
        {
            case Comparison.Greater:
                isInDistance = distance > controller.chaseMaxDistace;
                break;
            case Comparison.Equal:
                isInDistance = distance == controller.chaseMaxDistace;
                break;
            case Comparison.Lower:
                isInDistance = distance < controller.chaseMaxDistace;
                break;
        }


        return isInDistance;
    }

    public enum Comparison
    {
        Greater, Equal, Lower
    }
}
