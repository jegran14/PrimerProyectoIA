using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "StateMachine/NewState")]
public class State : ScriptableObject
{
    public Action[] actions;
    public Color sceneGizmoColor = Color.grey;

    public void UpdateState(AIController controller)
    {
        DoActions(controller);
    }

    private void DoActions(AIController controller)
    {
        for(int i = 0; i < actions.Length; i++)
        {
            actions[i].Act(controller);
        }
    }
}
