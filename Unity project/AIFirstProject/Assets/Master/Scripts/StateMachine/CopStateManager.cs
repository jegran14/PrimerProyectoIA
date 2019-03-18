using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CopStateMachine : MonoBehaviour
{
    private StateMachine stateMachine = new StateMachine();

    private void Start()
    {
        this.stateMachine.ChangeState(new State_Example());
        //aqui inicializariamos con el state Patrulla
    }

    private void Update()
    {
        this.stateMachine.ExecuteStateUpdate();
    }

}
