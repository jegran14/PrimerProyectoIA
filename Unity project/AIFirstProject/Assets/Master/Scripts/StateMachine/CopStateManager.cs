﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[RequireComponent(typeof(AIMovementController))]
[RequireComponent(typeof(StateMachine))]

//Controlador IA de las entidades que se vayan a mover
public class CopStateManager : AIController
{
    [Header("Debug Options")]
    public bool showGizmos = true;
    [Space]

    //------------------------ COMPONENTES NECESARIOS -----------------------------------
    private Animator anim;
    private StateMachine fsm;
    private CharacterMovement movementController;
    //-----------------------------------------------------------------------------------

    //-------------------------- Character parameters ----------------------------------
    [Header("Propiedades sensores")]
    [Tooltip("Radio del cono de vision (Distancia de vision)")]
    [SerializeField] private float _coneViewRadius = 5f;
    [Tooltip("Angulo del cono de vision")]
    [SerializeField] private float _coneViewAngle = 70f;
    [Tooltip("Radio del area")]
    [SerializeField] private float _closeAreaRadius = 1f;

    [Header("Propiedades patrulla")]
    [Tooltip("Puntos de ruta del personaje")]
    [SerializeField] private Transform[] _wayPoints;

    [Header("Propiedades persecución")]
    [Tooltip("Distancia maxima que se alejará el personaje de su area de patrulla")]
    [SerializeField] private float _chasingMaxDistance = 10f;

    //Posicion del item a perseguir
    private Transform _chasingTarget;
    //Punto de patrulla actual del personaje
    private int _currentWaypointIndex;
    //Centro del area de patrulla
    private Vector3 _patrolMiddlePoint;
    //-----------------------------------------------------------------------------------

    #region pathFindingProperties
    //----------------------    VARIABLES PARA EL PATHFINDING   ------------------------
    [Tooltip("Distancia minima del punto para que el personaje mire al punto")]
    public float turnDist = 3;  
    //Distancia minima que se tiene que mover el objetivo para que se recalcule el camino
    private const float pathUpdateThreshold = 0.5f;
    //Valor al cuadrado del valor minimo del pathUpdateThreshold util para calculos matematicos
    private float sqrMoveThreshold;
    //Camino de nodos a seguir
    private Path path;
    //Indice del punto del camino al que nos estamos moviendo
    private int pathIndex;
    //-----------------------------------------------------------------------------------
   

    //Posición a la que se está moviendo el personaje
    private Vector3 targetPos;
    //La entidad esta esperando a que le devielvan el camino
    private bool isWaitingForPath;
    #endregion

    #region gettersAndSetters
    //------------------------- SETTERS Y GETERS HEREDADOS DE LA CLASE PADRE -----------------------
    public override float coneViewRadius => _coneViewRadius;

    public override float coneViewAngle => _coneViewAngle;

    public override float closeAreaRadius => _coneViewRadius;

    public override Transform[] wayPoints => _wayPoints;

    public override float chasingMaxDistance => _chasingMaxDistance;

    public override int currentWaypointIndex { get => _currentWaypointIndex; set => _currentWaypointIndex = value; }

    public override Transform chasingTarget { get => _chasingTarget; set => _chasingTarget = value; }

    public override Vector3 patrolMiddlePoint => _patrolMiddlePoint;

    //-----------------------------------------------------------------------------------------------
    #endregion
    private void Start()
    {
        targetPos = (wayPoints.Length <= 0) ? transform.position : wayPoints[currentWaypointIndex].position;

        _patrolMiddlePoint = CalculateMiddlePoint(wayPoints);
        sqrMoveThreshold = pathUpdateThreshold * pathUpdateThreshold;
        _currentWaypointIndex = 0;

        //Inicializar componentes
        fsm = GetComponent<StateMachine>();
        anim = GetComponent<Animator>();
        movementController = GetComponent<CharacterMovement>();

        PathRequest(targetPos);

        fsm.controller = this;
    }


    /// <summary>
    /// Mover el personaje a un punto concreto
    /// </summary>
    /// <param name="point">Punto al que se debe mover el personaje</param>
    public override void MoveTo(Vector3 position)
    {
        if (isWaitingForPath || path == null)
            return;

        //Comprobar si el punta al que se quiere llegar es diferente al anterior
        if ((position - targetPos).sqrMagnitude > sqrMoveThreshold)
        {
            //En caso de que estemos moviendonos a un punto differente, hay que recalcular el camino
            PathRequest(position);
        }

        anim.SetFloat("Speed", 0); //Hacer que la animación este en iddle por si acaso el personaje no se moviera
        
        //Solo moverse en caso de que el index este dentro del tamaño del array
        if (!isWaitingForPath && pathIndex <= path.finishLineIndex)
            Move();
    }

    /// <summary>
    /// Pedir camino al PathFinding manager
    /// </summary>
    /// <param name="target">Posicion a la que se quiere llegar</param>
    private void PathRequest(Vector3 target)
    {
        isWaitingForPath = true;

        /*if (Time.timeSinceLevelLoad < 0.3f)
            return;*/

       PathRequestManager.RequestPath(new PathRequest(transform.position, target, OnPathFound));
       targetPos = target;
    }

    /// <summary>
    /// Realizarmovimiento del personaje
    /// </summary>
    private void Move()
    {  
        Vector2 pos2D = new Vector2(transform.position.x, transform.position.z);

        while (path.turnBoundaries[pathIndex].HasCrossedLine(pos2D))
        {
            if (pathIndex > path.finishLineIndex)
            {
                return;
            }
            else
            {
                pathIndex++;
                if (pathIndex > path.finishLineIndex)
                    return;
            }
        }

        
        movementController.Turn(path.lookPoints[pathIndex] - transform.position); //Girarlo en a direccion de movimiento
        movementController.Move(transform.forward); //Mover personaje a la posicion que queremos

        anim.SetFloat("Speed", 1); //Hacer que la animación camine
    }

    /// <summary>
    /// Transición a un nuevo estado
    /// </summary>
    /// <param name="nextState">Referencia al nuevo estado</param>
    public override void TransitionToState(State nextState)
    {
        fsm.TransitionToState(nextState);
        //Add if necessary a animation change for the animator
    }

    /// <summary>
    /// Se ha encontrado un camino al punto deseado
    /// </summary>
    /// <param name="newPath">Camino nuevo</param>
    /// <param name="pathSuccesful">Si el camino es correcto o no</param>
    public void OnPathFound(Vector3[] newPath, bool pathSuccesful)
    {
        if (pathSuccesful)
        {
            path = new Path(newPath, transform.position, turnDist);
            pathIndex = 0;
        }

        isWaitingForPath = false;
    }

    /// <summary>
    /// Comprobar si la entidad se encuentra en el posion del mapa deseada
    /// </summary>
    /// <returns>Devuelve si se encuentra en la posicion del mapa deseada o no</returns>
    public override bool IsAtTargetPos()
    {
        bool isAtTargetPos = false;
        Vector2 pos2D = new Vector2(transform.position.x, transform.position.z);

        if (path != null && !isWaitingForPath)
        {
            isAtTargetPos = pathIndex > path.finishLineIndex;
        }

        return isAtTargetPos;
    }

    /// <summary>
    /// Calcular la posicion media dada una lista de posiciones
    /// </summary>
    /// <param name="points">Lista de posiciones (Transform)</param>
    /// <returns></returns>
    private Vector3 CalculateMiddlePoint(Transform[] points)
    {
        float x = 0;
        float y = transform.position.y;
        float z = 0;

        for(int i = 0; i < points.Length; i++)
        {
            x += points[i].position.x;
            z += points[i].position.z;
        }

        x /= points.Length;
        z /= points.Length;

        return new Vector3(x, y, z);

    }

    /// <summary>
    /// Debug en el editor del camino a seguir de la entidad
    /// </summary>
    public void OnDrawGizmos()
    {
        if (showGizmos)
        {
            if (path != null)
            {
                path.DrawWithGizmos();
            }

            if (patrolMiddlePoint != null)
            {
                Gizmos.color = Color.blue;
                Gizmos.DrawWireSphere(patrolMiddlePoint, chasingMaxDistance);
            }
        }
    }
}
