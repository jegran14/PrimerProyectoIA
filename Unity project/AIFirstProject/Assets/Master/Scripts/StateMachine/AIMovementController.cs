using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Clase encargada de los steering behaviors
[RequireComponent(typeof(CharacterMovement))]
public class AIMovementController : MonoBehaviour
{
    public bool showGizmos = false;
    public bool isWalking = false;

    #region moveProperties
    //------------------------- PROPIEDADES DE MOVIMIENTO -------------------------------
    [Header("Propiedas de movimiento")]
    [Tooltip("Velocidad de movimiento del personaje caminando")]
    [SerializeField] private float _walkingSpeed = 3f;
    [Tooltip("Velocidad de movimiento delpersonaje corriendo")]
    [SerializeField] private float _runningSpeed = 8f;
    [Tooltip("Velocidad a la que se gira el personaje")]
    [SerializeField] private float _turningSpeed = 5f;
    //-----------------------------------------------------------------------------------
    #endregion
    #region steeringBehaviorsProperties
    //----------------------- PROPIEDADES PARA LAS STEERING BEHAVIORS -------------------
    [Header("Propiedades steering behaviors")]
    [Tooltip("Máscara de los obstaculos para el personaje")]
    [SerializeField] private LayerMask _obstacleMask;
    [Tooltip("Radio de colisión del personaje")]
    [SerializeField] private float _characterRadius = 0.3f;
    [Tooltip("Angulo del cono para comprobar las colisiones")]
    [SerializeField] private float _collisionConeAngle = 60f;
    //-----------------------------------------------------------------------------------
    #endregion

    #region pathFindingProperties
    //----------------------    VARIABLES PARA EL PATHFINDING   ------------------------
    [Tooltip("Distancia minima del punto para que el personaje mire al punto")]
    [SerializeField] float _turnDist = 3;
    //Distancia minima que se tiene que mover el objetivo para que se recalcule el camino
    private const float _pathUpdateThreshold = 0.5f;
    //Valor al cuadrado del valor minimo del pathUpdateThreshold util para calculos matematicos
    private float _sqrMoveThreshold;
    //Camino de nodos a seguir
    private Path _path;
    //Indice del punto del camino al que nos estamos moviendo
    private int _pathIndex;
    //-----------------------------------------------------------------------------------


    //Posición a la que se está moviendo el personaje
    private Vector3 _targetPos;
    //La entidad esta esperando a que le devielvan el camino
    private bool _isWaitingForPath;
    #endregion

    private CharacterMovement charMovement;

    //----------------------------- GETTERS Y SETTERS -----------------------------------
    public float collisionConeAngle { get { return _collisionConeAngle; } }
    //-----------------------------------------------------------------------------------

    private void Start()
    {
        charMovement = GetComponent<CharacterMovement>();

        _sqrMoveThreshold = _pathUpdateThreshold * _pathUpdateThreshold;
    }

    #region pathFindingFunctions
    public void SetTarget(Vector3 target)
    {
        //Comprobar si el punta al que se quiere llegar es diferente al anterior
        if (_targetPos == null || (target - _targetPos).sqrMagnitude > _sqrMoveThreshold)
        {
            //En caso de que estemos moviendonos a un punto differente, hay que recalcular el camino
            PathRequest(target);
        }
    }

    public void MoveTorwards(Vector3 position, MovementTypes type)
    {
        SetTarget(position);
 

        if (_path != null && (_pathIndex < _path.finishLineIndex || !_isWaitingForPath))
        {
            float speed = (type == MovementTypes.Walk) ? _walkingSpeed : _runningSpeed;
            Move(speed);
        }
    }

    /// <summary>
    /// Realizarmovimiento del personaje
    /// </summary>
    private void Move(float moveSpeed)
    {
        Vector2 pos2D = new Vector2(transform.position.x, transform.position.z);

        while (_path.turnBoundaries[_pathIndex].HasCrossedLine(pos2D))
        {
            if (_pathIndex > _path.finishLineIndex)
            {
                return;
            }
            else
            {
                _pathIndex++;
                if (_pathIndex > _path.finishLineIndex)
                {
                    isWalking = false;
                    return;
                }
            }
        }

        //Direccion en la que seencuentra el siguiente punto a moverse
        Vector3 pathDir = _path.lookPoints[_pathIndex] - transform.position;
        //Direccion en la que va a tener que rotar  el personaje
        Vector3 direction;
        //Comprobar si el personaje va a collisionar
        bool isColliding = TestObstacles(pathDir, out direction);

        charMovement.Turn(direction, _turningSpeed); //Girarlo en a direccion de movimiento

        if(!testDirection(direction))
            charMovement.Move(transform.forward, moveSpeed); //Mover personaje a la posicion que queremos

        isWalking = true;
    }

    /// <summary>
    /// Pedir camino al PathFinding manager
    /// </summary>
    /// <param name="target">Posicion a la que se quiere llegar</param>
    private void PathRequest(Vector3 target)
    {
        _isWaitingForPath = true;

        /*if (Time.timeSinceLevelLoad < 0.3f)
            return;*/

        PathRequestManager.RequestPath(new PathRequest(transform.position, target, OnPathFound));
        _targetPos = target;
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
            _path = new Path(newPath, transform.position, _turnDist);
            _pathIndex = 0;
        }

        _isWaitingForPath = false;
    }

    /// <summary>
    /// Comprobar si el personaje se encuentra al final del path
    /// </summary>
    /// <returns></returns>
    public bool IsAtEndPosition()
    {
        bool isAtTargetPos = false;
        Vector2 pos2D = new Vector2(transform.position.x, transform.position.z);

        if (_path != null && !_isWaitingForPath)
        {
            isAtTargetPos = _pathIndex > _path.finishLineIndex;
        }

        return isAtTargetPos;
    }
    #endregion

    #region steeringBehaviorFunctions
    /// <summary>
    /// Comprobar si la dirección de movimiento esta dentro del rango permitido
    /// </summary>
    /// <param name="dir">Direccion de movimiento</param>
    /// <returns>Devuelve si se ha producido una colisión o no</returns>
    private bool testDirection(Vector3 dir)
    {
        float angle = Vector3.SignedAngle(transform.forward, dir, Vector3.up);

        if (Mathf.Abs(angle) > 90f)
            return true;

        return false;
    }

    /// <summary>
    /// Comprobar si hay algun obstaculo en el camino
    /// </summary>
    /// <param name="dir">Direccion en  la que se encuentra el proximo punto de ruta</param>
    /// <param name="turnDir">Referencia al vector direccion para la rotacion</param>
    /// <returns></returns>
    private bool TestObstacles(Vector3 dir, out Vector3 turnDir)
    {
        //Calcular direcciones de los rayos
        Vector3 rightRayDir = Quaternion.AngleAxis(_collisionConeAngle, Vector3.up) * transform.forward;
        Vector3 leftRayDir = Quaternion.AngleAxis(-_collisionConeAngle, Vector3.up) * transform.forward;
        //Relizar raycasts para comprobar si hay collision
        bool rightRayColision = Physics.Raycast(transform.position, rightRayDir, 1.3f, _obstacleMask);
        bool leftRayColision = Physics.Raycast(transform.position, leftRayDir, 1.3f, _obstacleMask);

        //Si no hay colision devolver direccion original
        if(!rightRayColision && !leftRayColision) 
        {
            turnDir = dir.normalized;
            return false;
        }

        //Si nb hay colision a la derecha pero si a la izquierda, asignar direccion de rotacion de la derecha
        if (!rightRayColision && leftRayColision)
            turnDir = rightRayDir;
        else  //En elcaso contrario devolver direccion de rotacion izquierda
            turnDir = leftRayDir;

        return true;
    }
    #endregion

    public void OnDrawGizmos()
    {
        if(showGizmos)
        {
            _path.DrawWithGizmos();
        }
    }
}
