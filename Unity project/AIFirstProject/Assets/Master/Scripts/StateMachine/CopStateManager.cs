using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(StateMachine))]
public class CopStateManager : AIController
{
    public float viewRadious = 1f;

    private StateMachine stateMachine;

    private void Awake()
    {
        stateMachine = GetComponent<StateMachine>();
    }

    public override void MoveTo(Vector3 point)
    {
        //Move character
    }
}
