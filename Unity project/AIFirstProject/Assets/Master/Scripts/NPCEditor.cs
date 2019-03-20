using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(StateMachine))]
public class NPCEditor : Editor
{
    private void OnSceneGUI()
    {
        StateMachine fsm = (StateMachine)target;

        if(fsm.controller != null && fsm.currentState != null)
        {
            AIController fow = fsm.controller;

            Handles.color = fsm.currentState.sceneGizmoColor;
            Handles.DrawWireArc(fow.transform.position, Vector3.up, Vector3.forward, 360, fow.viewRadius);
            Vector3 viewAngleA = fow.DirFromAngle(-fow.viewAngle / 2, false);
            Vector3 viewAngleB = fow.DirFromAngle(fow.viewAngle / 2, false);

            Handles.DrawLine(fow.transform.position, fow.transform.position + viewAngleA * fow.viewRadius);
            Handles.DrawLine(fow.transform.position, fow.transform.position + viewAngleB * fow.viewRadius);
            Handles.DrawSolidArc(fow.transform.position, Vector3.up, viewAngleA, fow.viewAngle, fow.viewRadius);

            if(fow.chaseTarget != null)
            {
                Handles.color = Color.red;
                Handles.DrawLine(fow.transform.position, fow.chaseTarget.position);
            }
        }
    }
}
 