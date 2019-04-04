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

            //Distancia máxima de visión
            Handles.color = fsm.currentState.sceneGizmoColor;
            Handles.DrawWireArc(fow.transform.position, Vector3.up, Vector3.forward, 360, fow.viewMaxRadius);
            Vector3 viewAngleA = fow.DirFromAngle(-fow.viewAngle / 2, false);
            Vector3 viewAngleB = fow.DirFromAngle(fow.viewAngle / 2, false);

            //Cono de visión
            Handles.DrawLine(fow.transform.position, fow.transform.position + viewAngleA * fow.viewMaxRadius);
            Handles.DrawLine(fow.transform.position, fow.transform.position + viewAngleB * fow.viewMaxRadius);
            Handles.DrawSolidArc(fow.transform.position, Vector3.up, viewAngleA, fow.viewAngle, fow.viewMaxRadius);

            //Distancia mínima de visión
            Handles.DrawSolidArc(fow.transform.position, Vector3.up, Vector3.forward, 360, fow.viewMinRadius);

            if(fow.chaseTarget != null)
            {
                Handles.color = Color.red;
                Handles.DrawLine(fow.transform.position, fow.chaseTarget.position);
            }
        }
    }
}
 