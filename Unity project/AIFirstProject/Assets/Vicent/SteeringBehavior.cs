using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class SteeringBehavior : MonoBehaviour
{
    public float maxMovementSpeed = 5f;
    public float maxTurningSpeed = 10f;

    public float maxAcceleration = 2f;

    public float stoppingDistance = 1f;
    public float slowRadius = 2.5f;
    private float timeToTarget = 0.5f;

    public Transform target;

    private Rigidbody rb;

    private Vector3 currentVelocity;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();

        StartCoroutine(WalkingRoutine());
    }

    private IEnumerator WalkingRoutine()
    {
        bool walking = true;

        while (walking)
        {
            SteeringOutput seekOutput = Arrive(target.position);

            
            if(seekOutput.linear != Vector3.zero)
            {
                //Rotar el personaje en la direccion de la velocidad
                Quaternion rotation = Quaternion.LookRotation(seekOutput.linear.normalized);
                rb.MoveRotation(Quaternion.Slerp(rb.rotation, rotation, maxTurningSpeed * Time.deltaTime));
            }

            //Mover al personaje en la  direccion de la velocidad
            Vector3 newPos = rb.position + seekOutput.linear * Time.deltaTime;
            rb.MovePosition(newPos);

            currentVelocity = seekOutput.linear;

            yield return null;
        }
    }

    private void Move()
    {
        
    }

    private SteeringOutput Seek(Vector3 target)
    {
        //Cambiar la posicion del target en el  eje  y para  evitar errores
        target.y = transform.position.y;
        //Crear steering output
        SteeringOutput steering = new SteeringOutput();
        
        //Calcular direccion de movimiento
        steering.linear = target - transform.position;
        //Aplicarle acceleracion maxima al vector de  movimiento
        steering.linear = steering.linear.normalized * maxAcceleration;

        steering.angular = 0f;
        return steering;
    }

    private SteeringOutput Arrive(Vector3 target)
    {
        target.y = transform.position.y;
        SteeringOutput steering = new SteeringOutput();

        Vector3 direction = target - transform.position;
        float distance = Vector3.Distance(target, transform.position);

        if (distance < stoppingDistance)
        {
            steering.linear = Vector3.zero;
            return steering;
        }

        float targetSpeed = (distance > slowRadius) ? maxMovementSpeed : maxMovementSpeed * distance / slowRadius;

        Vector3 targetVelocity = direction.normalized * targetSpeed;

        steering.linear = (targetVelocity - currentVelocity) / timeToTarget;

        if (steering.linear.magnitude > maxAcceleration)
            steering.linear = steering.linear.normalized * maxAcceleration;

        steering.angular = 0;
        return steering;
    }

    #region SteeringStruct
    struct SteeringOutput
    {
        public Vector3 linear;
        public float angular;
    }
    #endregion
}
