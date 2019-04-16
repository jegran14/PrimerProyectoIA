using System.Collections;
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
    private AIMovementController movementController;
    //-----------------------------------------------------------------------------------
    
    #region characterParameters
    //-------------------------- Character parameters ----------------------------------
    [Header("Propiedades sensores")]
    [Tooltip("Radio del cono de vision (Distancia de vision)")]
    [SerializeField] private float _coneViewRadius = 5f;
    [Tooltip("Angulo del cono de vision")]
    [SerializeField] private float _coneViewAngle = 70f;
    [Tooltip("Radio del area de deteccion cercana")]
    [SerializeField] private float _closeAreaRadius = 1f;
    [Tooltip("Radio del area de alarma del personaje")]
    [SerializeField] private float _alarmAreaRadius = 25f;
    [SerializeField] private State _alarmState;

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
    #endregion

    #region gettersAndSetters
    //------------------------- SETTERS Y GETERS HEREDADOS DE LA CLASE PADRE -----------------------
    public override float coneViewRadius => _coneViewRadius;

    public override float coneViewAngle => _coneViewAngle;

    public override float closeAreaRadius => _closeAreaRadius;

    public override float alarmAreaRadius => _alarmAreaRadius;

    public override Transform[] wayPoints => _wayPoints;

    public override float chasingMaxDistance => _chasingMaxDistance;

    public override int currentWaypointIndex { get => _currentWaypointIndex; set => _currentWaypointIndex = value; }

    public override Transform chasingTarget { get => _chasingTarget; set => _chasingTarget = value; }

    public override Vector3 patrolMiddlePoint => _patrolMiddlePoint;

    //-----------------------------------------------------------------------------------------------

    //Scene manager
    private LevelChanger sceneChanger;

    #endregion
    private void Start()
    {
        _patrolMiddlePoint = CalculateMiddlePoint(wayPoints);
        _currentWaypointIndex = 0;

        //Inicializar componentes
        fsm = GetComponent<StateMachine>();
        anim = GetComponent<Animator>();
        movementController = GetComponent<AIMovementController>();

        movementController.SetTarget((wayPoints.Length <= 0) ? transform.position : wayPoints[currentWaypointIndex].position);

        fsm.controller = this;
    }


    /// <summary>
    /// Mover el personaje a un punto concreto
    /// </summary>
    /// <param name="point">Punto al que se debe mover el personaje</param>
    public override void MoveTo(Vector3 position, MovementTypes movementType)
    {
        movementController.MoveTorwards(position, movementType);

        float speedPercentage = movementController.currentSpeed/2;
        anim.SetFloat("Speed", speedPercentage);
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
    /// Comprobar si la entidad se encuentra en el posion del mapa deseada
    /// </summary>
    /// <returns>Devuelve si se encuentra en la posicion del mapa deseada o no</returns>
    public override bool IsAtTargetPos()
    {
        return movementController.IsAtEndPosition();
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
    /// Mirar en un direccion a una posicion en el espacio tridimensional
    /// </summary>
    /// <param name="point">Punto al que  debe mirar el personaje</param>
    public override void LookAt(Vector3 point)
    {

    }

    /// <summary>
    /// Recibir alarma de  otros guardias, o elementos
    /// </summary>
    /// <param name="targetPos">Posicion de la alarma</param>
    public override void SetAlarm(Transform chaseTarget)
    {
        chasingTarget = chaseTarget;
        TransitionToState(_alarmState);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.name == "Player")
        {
            sceneChanger = FindObjectOfType<LevelChanger>();
            sceneChanger.FadeToBlack();

            anim.SetFloat("Speed", 0);
            fsm.aiActive = false;
        }
    }

    /// <summary>
    /// Debug en el editor del camino a seguir de la entidad
    /// </summary>
    public void OnDrawGizmos()
    {
        if (showGizmos)
        {
            if (patrolMiddlePoint != null)
            {
                Gizmos.color = Color.blue;
                Gizmos.DrawWireSphere(patrolMiddlePoint, chasingMaxDistance);
            }
        }
    }
}
