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
    //Camino de waypoints que ha de seguir el guardia
    private Vector3[] path;
    //Indice del del siguiente punto que se ha de mover el guardia en su camino
    private int targetIndex;
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
        PathRequest(wayPoints[currentWayPoint].position);
    }

    /// <summary>
    /// Mover el personaje a un punto concreto
    /// </summary>
    /// <param name="point">Punto al que se debe mover el personaje</param>
    public override void MoveTo(Vector3 point)
    {
        if (path == null || isWaitingForPath)
            return;

        point.y = transform.position.y;

        //Comprobar si el punta al que se quiere llegar es diferente al anterior
        if (targetPos != point)
        {
            //En caso de que estemos moviendonos a un punto differente, hay que recalcular el camino
            PathRequest(point);
            targetPos = point;
        }

        //Solo moverse en caso de que el index este dentro del tamaño del array
        if(targetIndex < path.Length)
            Move();
    }

    /// <summary>
    /// Pedir camino al PathFinding manager
    /// </summary>
    /// <param name="pos">Posicion a la que se quiere llegar</param>
    private void PathRequest(Vector3 pos)
    {
        isWaitingForPath = true;
        PathRequestManager.RequestPath(transform.position, pos, OnPathFound);
    }

    /// <summary>
    /// Realizarmovimiento del personaje
    /// </summary>
    private void Move()
    {
        nextNodePos = path[targetIndex];
        nextNodePos.y = transform.position.y;

        if (transform.position == nextNodePos)
        {
            targetIndex++;
            if (targetIndex >= path.Length) //Si estamos donde queremos, no hacer nada
                return;
        }

        movementController.MoveTo(nextNodePos); //Mover personaje a la posicion que queremos
        movementController.Turn((nextNodePos - transform.position).normalized); //Girarlo en a direccion de movimiento

        anim.SetFloat("Speed", (nextNodePos - transform.position).magnitude); //Hacer que la animación camine
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
            path = newPath;
            targetIndex = 0;           
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

        if (path.Length > 0 && !isWaitingForPath)
        {
            Vector3 endPos = path[path.Length - 1];
            endPos.y = transform.position.y;

            isAtTargetPos = transform.position == endPos;
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
                for (int i = targetIndex; i < path.Length; i++)
                {
                    Gizmos.color = Color.black;
                    Gizmos.DrawCube(path[i], Vector3.one);

                    if (i == targetIndex)
                    {
                        Gizmos.DrawLine(transform.position, path[i]);
                    }
                    else
                    {
                        Gizmos.DrawLine(path[i - 1], path[i]);
                    }
                }
            }

            if (patrolMiddlePoint != null)
            {
                Gizmos.color = Color.blue;
                Gizmos.DrawWireSphere(patrolMiddlePoint, chaseMaxDistace);
            }
        }
    }
}
