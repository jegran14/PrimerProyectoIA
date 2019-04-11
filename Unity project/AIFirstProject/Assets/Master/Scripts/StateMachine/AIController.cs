using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Clase padre encargada de los controladores de todas las IA
public abstract class AIController : MonoBehaviour
{
    //----------------------------------------- SETTERS Y GETTERS DE LAS PROPIEDADES DE LOS HIJOS -------------------------
    public abstract float coneViewRadius {get; }
    public abstract float coneViewAngle { get; }
    public abstract float closeAreaRadius { get; }
    public abstract float alarmAreaRadius { get; }
    public abstract Transform[] wayPoints { get; }
    public abstract float chasingMaxDistance { get; }
    public abstract int currentWaypointIndex { get; set; }
    public abstract Transform chasingTarget { set; get; }
    public abstract Vector3 patrolMiddlePoint { get; }
    // ---------------------------------------------------------------------------------------------------------------------


    //---------------- FUNCIONES A IMPLEMENTAR DENTRO DE LOS CONTROLADORES HIJOS ---------------------------------------------------
    public abstract void MoveTo(Vector3 point, MovementTypes type); //Move character, every controller moves the character as they think they should
    public abstract void LookAt(Vector3 point); //Mirar en un  punto en concreto
    public abstract void TransitionToState(State nextState); //Transicionar estado
    public abstract void SetAlarm(Transform chaseTarget); //Recibir alarma de otros guardias
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
}
