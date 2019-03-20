using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StateMachine : MonoBehaviour
{
    public State currentState;
    public bool aiActive = false;

    [HideInInspector] public AIController controller;

    private void Update()
    {
        if (!aiActive)
            return;

        currentState.UpdateState(controller);
    }

    private void OnDrawGizmos()
    {
        if(currentState != null)
        {
            Gizmos.color = currentState.sceneGizmoColor;
            Gizmos.DrawWireSphere(transform.position, controller.viewRadius);
        }
    }
}
