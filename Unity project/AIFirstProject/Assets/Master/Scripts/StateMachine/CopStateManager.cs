using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Controlador IA del guardia
[RequireComponent(typeof(CharacterMovement))]
public class CopStateManager : AIController
{
    [Header("Debug Options")]
    public bool showGizmos = true;

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
    //Posicion a la que nos estamos moviendo
    private Vector3 nextNodePos;
    //La entidad esta esperando a que le devielvan el camino
    private bool isWaitingForPath;


    //Controlador de movimiento del personaje
    private CharacterMovement movementController;

    private void Start()
    {
        movementController = GetComponent<CharacterMovement>();

        //Inicializar primera posicion a la que moverse
        targetPos = wayPoints[currentWayPoint].position;
        pathIndex = 0;

        sqrMoveThreshold = pathUpdateThreshold * pathUpdateThreshold;

        PathRequest(targetPos);
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
        if (!isWaitingForPath && pathIndex < path.finishLineIndex)
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
            if (pathIndex == path.finishLineIndex)
            {
                break;
            }
            else
                pathIndex++;
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
            isAtTargetPos = path.turnBoundaries[path.turnBoundaries.Length - 1].HasCrossedLine(pos2D);
        }

        return isAtTargetPos;
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
                Gizmos.DrawWireSphere(patrolMiddlePoint, chaseMaxDistace);
            }
        }
    }
}
