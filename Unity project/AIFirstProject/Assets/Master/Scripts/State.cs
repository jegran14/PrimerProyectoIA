using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Clase base de los estados, esta compuesta por las acciones que puede realizar, y las decisiones que debe tomar
[CreateAssetMenu(menuName = "StateMachine/NewState")]
public class State : ScriptableObject
{
    [Tooltip("Lista de acciones que debe realizar el estado")]
    public Action[] actions;
    [Tooltip("Lisa de transiociones del estado")]
    public Transition[] transitions;
    [Tooltip("Color con el que se muestra el estado en el editor")]
    public Color sceneGizmoColor = Color.grey;

    public void UpdateState(AIController controller)
    {
        DoActions(controller);
        CheckTransiotions(controller);
    }
    /// <summary>
    /// Realizar las acciones asignadas al estado
    /// </summary>
    /// <param name="controller">Referencia al controlador de la IA</param>
    private void DoActions(AIController controller)
    {
        for(int i = 0; i < actions.Length; i++)
        {
            actions[i].Act(controller);
        }
    }

    private void CheckTransiotions(AIController controller)
    {
        for(int i = 0; i < transitions.Length; i++)
        {
            bool transitionSucceded = transitions[i].decision.Decide(controller);

            if (transitionSucceded)
                controller.TransitionToState(transitions[i].trueState);
            else
                controller.TransitionToState(transitions[i].falseState);
        }
    }
}
