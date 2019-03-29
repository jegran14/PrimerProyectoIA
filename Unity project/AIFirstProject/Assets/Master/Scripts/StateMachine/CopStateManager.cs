using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Controlador IA del guardia
[RequireComponent(typeof(CharacterMovement))]
public class CopStateManager : AIController
{
    //----------------------    VARIABLES PARA EL PATHFINDING   ------------------------
    //Camino de waypoints que ha de seguir el guardia
    private Vector3[] path;
    //Indice del del siguiente punto que se ha de mover el guardia en su camino
    private int targetIndex;
    //-----------------------------------------------------------------------------------

    //Posición a la que se está moviendo el personaje
    private Vector3 targetPos;
    //El personaje está esperando a que se le de un nuevo camino
    private bool isWaitingForPath = false;

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
        //Comprobar si el punto de patrulla es diferente punto que se ha pasado a la funcion
        if(targetPos != point && !isWaitingForPath)
        {
            //En caso de que estemos moviendonos a un punto differente, hay que recalcular el camino
            PathRequest(point);
            targetPos = point;
        }

        if (path.Length > 0 && targetIndex < path.Length)
            Move();
    }

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
        //En caso de que ya estemos donde queremos pondremos la velocidad del animator a 0 para el Iddle
        Vector3 targetPos = path[targetIndex];
        targetPos.y = this.transform.position.y;

        anim.SetFloat("Speed", 0f);

        if (transform.position == path[targetIndex])
        {
            targetIndex++;
            if (targetIndex >= path.Length)
                return;
        }

        movementController.MoveTo(targetPos);
        movementController.Turn((targetPos - transform.position).normalized);
        anim.SetFloat("Speed", 1f);
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

    public void OnPathFound(Vector3[] newPath, bool pathSuccesful)
    {
        if (pathSuccesful)
        {
            path = newPath;
            targetIndex = 0;
            isWaitingForPath = false;
        }
    }

    public void OnDrawGizmos()
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
    }

    public override bool IsAtTargetPos()
    {
        bool isAtTargetPos = false;

        if (!isWaitingForPath && path.Length > 0)
        {
            Vector3 targetPos = path[path.Length - 1];
            targetPos.y = transform.position.y;

            isAtTargetPos = transform.position == targetPos;
        }

        return isAtTargetPos;
    }
}
