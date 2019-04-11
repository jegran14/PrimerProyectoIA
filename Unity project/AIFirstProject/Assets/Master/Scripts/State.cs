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

    /// <summary>
    /// Comprobar transiciones del estado
    /// </summary>
    /// <param name="controller">Controlador de la IA</param>
    private void CheckTransiotions(AIController controller)
    {
        //Comprobar todas las transiciones
        for(int i = 0; i < transitions.Length; i++)
        {
            bool transitionSucceded = transitions[i].decision.Decide(controller);
            //Si la transicion es verdadera
            if (transitionSucceded)
            {
                //Realizar acciones necesarias para  la transicion
                foreach (Action action in transitions[i].trueActions)
                    action.Act(controller);
                //Transicion 
                controller.TransitionToState(transitions[i].trueState);
            }
            else //Si la transicion  es falsa
            {
                foreach (Action action in transitions[i].falseActions)
                    action.Act(controller);

                controller.TransitionToState(transitions[i].falseState);
            }
        }
    }
}
