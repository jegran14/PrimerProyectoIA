using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SteeringBehavior : MonoBehaviour
{


    public float rayDistance = 2f;

    private RaycastHit hit;
    private bool hasHit;


    private void Update()
    {
        hasHit = Physics.Raycast(transform.position, transform.forward, out hit, rayDistance);
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;

        if (hasHit)
        {
            Gizmos.DrawLine(transform.position, hit.point);

            Gizmos.DrawSphere(hit.point, 0.1f);

            float normalDistance = Vector3.Distance(transform.position, hit.point);
            normalDistance = rayDistance - normalDistance;

            Gizmos.DrawLine(hit.point, hit.point + hit.normal * rayDistance);
        }
        else
            Gizmos.DrawLine(transform.position, transform.position + transform.forward * rayDistance);
    }
}
