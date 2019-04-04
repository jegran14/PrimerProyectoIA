using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Clase padre encargada de los controladores de todas las IA
[RequireComponent(typeof(StateMachine))]
public abstract class AIController : MonoBehaviour
{
    [Header("Propiedades del area de vision")]
    [Tooltip("Radio de vision máximo del personaje")]
    public float viewMaxRadius = 3f;
    [Tooltip("Radio de visión mínimo del personaje")]
    public float viewMinRadius = 1f;
    [Tooltip("Angulo de vision del personaje en la m'axima distancia")]
    public float viewAngle = 90f;

    [Header("Propiedades de patrulla")]
    [Tooltip("Lista de waypoints a los que el NPC debe moverse en caso de patrulla")]
    public Transform[] wayPoints;
    [Tooltip("Distancia maxima a la que la entidad perseguira al jugador, antes de volver a la patrulla")]
    public float chaseMaxDistace = 7f;
    [HideInInspector]public int currentWayPoint; //WayPoint activo

    public Transform chaseTarget; //Referencia al target a perseguir
    [HideInInspector] public StateMachine fsm; //Maquina de estados
    protected Animator anim; //Animator

    //Punto medio entre todos los puntos de patrulla
    protected Vector3 patrolMiddlePoint;

    private void Awake()
    {
        fsm = GetComponent<StateMachine>();
        anim = GetComponent<Animator>();
        fsm.controller = this;

        currentWayPoint = 0;

        patrolMiddlePoint = CalculateMiddlePoint(GetPositionsFromTransform(wayPoints));
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

    private Vector3[] GetPositionsFromTransform(Transform[] transforms)
    {
        Vector3[] points = new Vector3[transforms.Length];

        for(int t = 0; t < transforms.Length; t++)
        {
            points[t] = transforms[t].position;
        }

        return points;
    }

    private Vector3 CalculateMiddlePoint(Vector3[] points)
    {
        float x = 0f;
        float y = transform.position.y;
        float z = 0f;

        for(int i = 0; i < points.Length; i++)
        {
            x += points[i].x;
            z += points[i].z;
        }

        x = x / points.Length;
        z = z / points.Length;

        return new Vector3(x, y, z);
    }

    public Vector3 PatrolMiddlePoint
    {
        get { return patrolMiddlePoint; }
    }
}
