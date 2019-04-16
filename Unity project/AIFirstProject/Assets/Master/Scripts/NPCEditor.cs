using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

#if UNITY_EDITOR
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
            Handles.DrawWireArc(fow.transform.position, Vector3.up, Vector3.forward, 360, fow.coneViewRadius);
            Vector3 viewAngleA = fow.DirFromAngle(-fow.coneViewAngle / 2, false);
            Vector3 viewAngleB = fow.DirFromAngle(fow.coneViewAngle / 2, false);

            //Cono de visión
            Handles.DrawLine(fow.transform.position, fow.transform.position + viewAngleA * fow.coneViewRadius);
            Handles.DrawLine(fow.transform.position, fow.transform.position + viewAngleB * fow.coneViewRadius);
            Handles.DrawSolidArc(fow.transform.position, Vector3.up, viewAngleA, fow.coneViewAngle, fow.coneViewRadius);

            //Distancia mínima de visión
            Handles.DrawSolidArc(fow.transform.position, Vector3.up, Vector3.forward, 360, fow.closeAreaRadius);

            if(fow.chasingTarget != null)
            {
                Handles.color = Color.red;
                Handles.DrawLine(fow.transform.position, fow.chasingTarget.position);
            }
        }
    }
}
#endif
