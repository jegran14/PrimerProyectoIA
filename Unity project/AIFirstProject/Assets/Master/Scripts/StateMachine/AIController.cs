using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Clase padre encargada de los controladores de todas las IA
public abstract class AIController : MonoBehaviour
{
    [Header("Propiedades del cono de vision")]
    [Tooltip("Radio de vision del personaje")]
    public float viewRadius = 1f;
    [Tooltip("Angulo de vision del personaje")]
    public float viewAngle = 90f;
    [Space]

    [Header("Propiedades de patrulla")]
    [Tooltip("Lista de waypoints a los que el NPC debe moverse en caso de patrulla")]
    public Transform[] wayPointList;
    [HideInInspector] public int nextWayPoint; //WayPoint activo
    [HideInInspector] public Transform chaseTarget;
    [HideInInspector] public StateMachine fsm;

    private void Awake()
    {
        fsm = GetComponent<StateMachine>();
        fsm.controller = this;
    }

    public abstract void MoveTo(Vector3 point); //Move character, every controller moves the character as they think they should

    public Vector3 DirFromAngle(float angleInDegrees, bool angleInGlobal)
    {
        if (!angleInGlobal)
            angleInDegrees += transform.eulerAngles.y;

        return new Vector3(Mathf.Sin(angleInDegrees * Mathf.Deg2Rad), 0f, Mathf.Cos(angleInDegrees * Mathf.Deg2Rad));
    }
}
