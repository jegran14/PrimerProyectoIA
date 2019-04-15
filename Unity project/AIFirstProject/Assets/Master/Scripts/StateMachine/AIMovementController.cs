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
    [SerializeField] private float _movingSpeed = 3f;
    public float movingSpeed { get { return _movingSpeed; } }
    [Tooltip("Velocidad de movimiento delpersonaje corriendo")]
    [SerializeField] private float _runningMultiplier = 2f;
    [Tooltip("Velocidad a la que se gira el personaje")]
    [SerializeField] private float _turningSpeed = 5f;
    //-----------------------------------------------------------------------------------
    #endregion

    #region steeringBehaviorsProperties
    //----------------------- PROPIEDADES PARA LAS STEERING BEHAVIORS -------------------
    [Header("Propiedades steering behaviors")]
    [Tooltip("Porcentaje aplicado de la velocidad máxima")]
    [SerializeField] private float _maxAcceleration = 1f;

    private float _currentMaxAcceleration;
    private Vector3 _currentVelocity;
    public float currentSpeed { get { return _currentVelocity.magnitude; } }

    [Tooltip("Distancia a la que el agente debe pararse")]
    [SerializeField] private float _stoppingDistance = 0.5f;
    [Tooltip("Distancia a la que el agente empieza a reducir la velocidad")]
    [SerializeField] private float _slowDistanceRadius = 1f;

    [Tooltip("Máscara de los obstaculos para el personaje")]
    [SerializeField] private LayerMask _obstacleMask;
    [Tooltip("Radio de colisión del personaje")]
    [SerializeField] private float _characterRadius = 0.3f;
    [Tooltip("Angulo del cono para comprobar las colisiones")]
    [SerializeField] private float _collisionDistance = 1f;
    //-----------------------------------------------------------------------------------

    //------------------ PROPIEDADES DETECCION DE OBSTACULOS ----------------------------
    //Radio del personaje en cordenadas globales
    private float _globalCharacterRadius;
    //Angulo de apertura minimo de los rayos para la colision en grados
    private float _minWiskersAngle;
    //Angulo actual de los rayos
    private float _currentWiskersAngle;
    //Ratio de cambio del angulo de los rayos laterales
    private float _angleIncrement = 5f;
    //-----------------------------------------------------------------------------------
    #endregion

    #region pathFindingProperties
    //----------------------    VARIABLES PARA EL PATHFINDING   ------------------------
    [Header("Propiedades PathFinding")]
    [Tooltip("Distancia minima del punto para que el personaje mire al punto")]
    [SerializeField] float _turnDist = 3;
    //Distancia minima que se tiene que mover el objetivo para que se recalcule el camino
    private const float _pathUpdateThreshold = 0.5f;
    //Valor al cuadrado del valor minimo del pathUpdateThreshold util para calculos matematicos
    private float _sqrMoveThreshold;
    //Camino de nodos a seguir
    private Path _path;
    //Indice del punto del camino al que nos estamos moviendo
    [SerializeField]private int _pathIndex;
    //-----------------------------------------------------------------------------------


    //Posición a la que se está moviendo el personaje
    private Vector3 _targetPos;
    //La entidad esta esperando a que le devielvan el camino
    private bool _isWaitingForPath;
    #endregion

    private AIController _controller;
    private CharacterMovement _charMovement;
    private Rigidbody _rb;


    private void Start()
    {
        _charMovement = GetComponent<CharacterMovement>();
        _controller = GetComponent<AIController>();
        _rb = GetComponent<Rigidbody>();

        _sqrMoveThreshold = _pathUpdateThreshold * _pathUpdateThreshold;
        //La multiplicacion por dos del radio al hacerlo global, es para añadirle un offset que nos ahorrara fallos de deteccion
        _globalCharacterRadius = _characterRadius * 2 * transform.localScale.x;
        //Calcular angulo minimo
        _minWiskersAngle = Mathf.Asin(_globalCharacterRadius / _collisionDistance) * Mathf.Rad2Deg;
        _currentWiskersAngle = _minWiskersAngle;

        _currentMaxAcceleration = _maxAcceleration;
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

        if (_path != null && (_pathIndex < _path.lookPoints.Length || !_isWaitingForPath))
        {
            _currentMaxAcceleration = (type == MovementTypes.Walk) ? _maxAcceleration : _runningMultiplier;
            Move();
        }
        else isWalking = false;
    }

    /// <summary>
    /// Realizar movimiento del personaje
    /// </summary>
    private void Move()
    {
        if (Vector3.Distance(transform.position, _path.lookPoints[_pathIndex]) <= _stoppingDistance)
        {
            _pathIndex++;

            if (_pathIndex > _path.finishLineIndex)
                return;
        }

        Vector3 seekVelocity = SeekBehavior(_path.lookPoints[_pathIndex]);

        Vector3 newPos = _rb.position + seekVelocity * _movingSpeed * Time.deltaTime;
        _rb.MovePosition(newPos);

        Quaternion newRotation = Quaternion.LookRotation(seekVelocity.normalized);
        _rb.MoveRotation(Quaternion.Lerp(_rb.rotation, newRotation, _turningSpeed * Time.deltaTime));

        _currentVelocity = seekVelocity;
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
        _pathIndex = 0;
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
    /// Comportamiento de busqueda basico
    /// </summary>
    /// <param name="targetPos">Posicion a la que el personaje quiere moverse</param>
    /// <returns>Devuelve la velocidad a la que deberia moverse</returns>
    private Vector3 SeekBehavior(Vector3 target)
    {
        target.y = transform.position.y;

        Vector3 direction = target - transform.position;
        float distance = Vector3.Distance(target, transform.position);

        return direction.normalized * _currentMaxAcceleration;
    }

    /// <summary>
    /// Calcular velocidad para evitar obstaculos
    /// </summary>
    /// <param name="target">Punto al que quere mover</param>
    /// <returns>Velocidad a la que el personaje debe moverse para evitur los obstaculos</returns>
    private Vector3 AvoidObstacles(Vector3 target)
    {
        target.y = transform.position.y;

        Vector3 direction = target - transform.position;
        float distance = Vector3.Distance(target, transform.position);


        Vector3 turnDir;
        //Calcular direcciones de los rayos
        Vector3 rightRayDir = _controller.DirFromAngle(_minWiskersAngle, false);
        Vector3 leftRayDir = _controller.DirFromAngle(-_minWiskersAngle, false);

        RaycastHit hitRight;
        RaycastHit hitLeft;

        //Relizar raycasts para comprobar si hay collision
        bool rightRayColision = Physics.Raycast(transform.position, rightRayDir, out hitRight, _collisionDistance, _obstacleMask);
        bool leftRayColision = Physics.Raycast(transform.position, leftRayDir, out hitLeft, _collisionDistance, _obstacleMask);

        //Si no hay colision no devolver ninguna velocidad
        if(!rightRayColision && !leftRayColision) 
        {
            //Decrementar angulo de los rayos laterales
            //_currentWiskersAngle -= _angleIncrement;
            //Comprobar que  no  sobrepase el minimo
            //_currentWiskersAngle = (_currentWiskersAngle < _minWiskersAngle) ? _minWiskersAngle : _currentWiskersAngle;

            return Vector3.zero;
        }

        //Si nb hay colision a la derecha pero si a la izquierda, asignar direccion de rotacion de la derecha
        if (!rightRayColision && leftRayColision)
            turnDir = rightRayDir;
        else  //En elcaso contrario devolver direccion de rotacion izquierda
            turnDir = leftRayDir;

        //Incrementar  angulo de los rayos laterales
        //_currentWiskersAngle += _angleIncrement;
        //Comprobar que no se pase del maximo, en este  caso, 90 grados
        //_currentWiskersAngle = (_currentWiskersAngle > 90f) ? 90f : _currentWiskersAngle;

        return turnDir  * _maxAcceleration;
    }
    #endregion

    public void OnDrawGizmos()
    {
        if(showGizmos)
        {
            if (_path != null && _pathIndex < _path.lookPoints.Length)
            {
                Gizmos.color = Color.black;
                Gizmos.DrawLine(transform.position, _path.lookPoints[_pathIndex]);
                _path.DrawWithGizmos();
            }

            if(_controller != null)
            {
                //Calcular direcciones de los rayos
                Vector3 rightRayDir = _controller.DirFromAngle(_currentWiskersAngle, false);
                Vector3 leftRayDir = _controller.DirFromAngle(-_currentWiskersAngle, false);

                Gizmos.color = Color.red;
                Gizmos.DrawLine(transform.position, transform.position + rightRayDir);
                Gizmos.DrawLine(transform.position, transform.position + leftRayDir);
            }
        }
    }
}
