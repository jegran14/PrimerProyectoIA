using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Clase padre encargada de los controladores de todas las IA
[RequireComponent(typeof(StateMachine))]
public abstract class AIController : MonoBehaviour
{
    [Header("Propiedades del cono de vision")]
    [Tooltip("Radio de vision del personaje")]
    public float viewRadius = 1f;
    [Tooltip("Angulo de vision del personaje")]
    public float viewAngle = 90f;

    [Header("Propiedades de patrulla")]
    [Tooltip("Lista de waypoints a los que el NPC debe moverse en caso de patrulla")]
    public Transform[] wayPoints;
    [HideInInspector]public int currentWayPoint; //WayPoint activo

    [HideInInspector] public Transform chaseTarget; //Referencia al target a perseguir
    [HideInInspector] public StateMachine fsm; //Maquina de estados
    protected Animator anim; //Animator

    private void Awake()
    {
        fsm = GetComponent<StateMachine>();
        anim = GetComponent<Animator>();
        fsm.controller = this;

        currentWayPoint = 0;
    }

    //---------------- FUNCIONES A IMPLEMENTAR DENTRO DE LOS CONTROLADORES HIJOS ---------------------------------------------------
    public abstract void MoveTo(Vector3 point); //Move character, every controller moves the character as they think they should
    public abstract void TransitionToState(State nextState); //Transicionar estado
    public abstract bool IsAtTargetPos(); //Comprobar si la entidad se encuentra en el lugar desaeado
    //-------------------------------------------------------------------------------------------------------------------------------

    /// <summary>
    /// Calcular el vector direccion a partir de un angulo representado en grados
    /// </summary>
    /// <param name="angleInDegrees">Angulo desde el que se quiere conseguir la direccion</param>
    /// <param name="angleInGlobal">¿Esta el angulo en coordenadas globales?</param>
    /// <returns>Devuelve vector direccion en formato Vector3</returns>
    public Vector3 DirFromAngle(float angleInDegrees, bool angleInGlobal)
    {
        if (!angleInGlobal)
            angleInDegrees += transform.eulerAngles.y;

        return new Vector3(Mathf.Sin(angleInDegrees * Mathf.Deg2Rad), 0f, Mathf.Cos(angleInDegrees * Mathf.Deg2Rad));
    }

    /// <summary>
    /// Recibir waypoint de la lista de waypoints
    /// </summary>
    /// <param name="index">Indice del waypoint dentro de la lista</param>
    /// <returns></returns>
    public Transform GetWayPoint(int index)
    {
        return wayPoints[index];
    }
}
