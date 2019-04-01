using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Comprobar si el jugador se encuentra en el rango de vision
[CreateAssetMenu(menuName = "StateMachine/Decisions/LookDecision")]
public class LookDecision : Decision
{
    [Tooltip("Layer del elemento a buscar, la del jugador es máscara Target")]
    public LayerMask targetMask;
    [Tooltip("Layer de los elementos que pueden obstruir la visión")]
    public LayerMask obstacleMask;

    public override bool Decide(AIController controller)
    {
        bool targetVisible = Look(controller);
        return Look(controller);
    }

    /// <summary>
    /// Función encargada de realizar la busqueda del target escogido
    /// </summary>
    /// <param name="controller">Referencia al controlador de la inteligencia artificial</param>
    /// <returns></returns>
    private bool Look (AIController controller)
    {
        //Buscar si el target se encuentra dentro del area de vision
        Collider[] targetsInViewRadius = Physics.OverlapSphere(controller.transform.position, controller.viewRadius, targetMask);

        //Si se ha encontrado algun target, comprobar que no haya ningun obstaculo en medio
        for (int i = 0; i < targetsInViewRadius.Length; i++)
        {
            Transform target = targetsInViewRadius[i].transform;
            Vector3 dirToTarget = (target.position - controller.transform.position).normalized; //Calcular direccion al target
            //Si el target esta dentro del angulo de vision procedemos a encontrar si hay algun obstaculo de por medio
            if (Vector3.Angle(controller.transform.forward, dirToTarget) < controller.viewAngle / 2)
            {
                float distToTarget = Vector3.Distance(controller.transform.position, target.position); //Distancia hasta el target
                //Comprobar si entre la entidad y su objetivo hay algun obstaculo, si no hay ningun obstaculo, se le asigna al controller y devolvemos true
                if (!Physics.Raycast(controller.transform.position, dirToTarget, distToTarget, obstacleMask))
                {
                    controller.chaseTarget = target;
                    return true;
                }
            }
        }
        //Si no se ha encontrado ningun target, o esta obstruido por algun obstaculo, se devuelve false;
        return false;
    }
}
