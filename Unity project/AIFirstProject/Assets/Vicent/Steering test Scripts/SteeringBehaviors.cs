using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SteeringBehaviors : MonoBehaviour
{
    public float maxSpeed = 3f;
    public float maxForce = 1f;

    public Transform target;

    private Vector3 currentVel;
    private bool walking = true;

    private Rigidbody rb;

    private Ray forceDebugRay;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        currentVel = Vector3.zero;

        StartCoroutine("WalkRoutine");
    }

    private IEnumerator WalkRoutine()
    {
        while (walking)
        {
            
            Vector3 movement = SeekBehavior(target.position);


            rb.AddForce(movement);

            yield return null;
        }
    }

    private Vector3 SeekBehavior (Vector3 targetPos)
    {
        Vector3 desiredVel = targetPos - transform.position;
        desiredVel = desiredVel.normalized * maxSpeed;

        Vector3 steering = desiredVel - currentVel;

        if (steering.magnitude > maxForce)
            steering = steering.normalized * maxForce;

        currentVel = steering;
        steering.y = 0;

        forceDebugRay = new Ray(transform.position, steering);

        return steering;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawRay(transform.position, transform.forward);

        Gizmos.color = Color.red;
        Gizmos.DrawRay(transform.position, forceDebugRay.direction);
    }
}
