using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Clase encargada de la máquina de estados
public class StateMachine : MonoBehaviour
{
    [Tooltip("Estado inicial del personaje")]
    public State initialState;
    [Tooltip("Activar/Desactivar maquina de esados")]
    public bool aiActive = false;
    [HideInInspector]
    public State currentState; //Estado actual de la máquina de estados

    [HideInInspector] public AIController controller; //Referencia al controlador de la IA

    private void Start()
    {
        currentState = initialState;
    }

    private void Update()
    {
        if (!aiActive)
            return;

        currentState.UpdateState(controller);
    }

    public void TransitionToState(State nextState)
    {
        if(currentState != nextState)
        {
            currentState = nextState;
        }
    }
}
