using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class CharacterMovement : MonoBehaviour
{
    [Tooltip("Velocidad de movimiento del jugador")]
    public float movementSpeed = 6f;
    [Tooltip("Velocidad de rotacion del jugador")]
    public float turnSpeed = 20f;

    private Rigidbody rb;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    /// <summary>
    /// Moves the character in the desired direction
    /// </summary>
    /// <param name="dir">Movement direction</param>
    public void Move(Vector3 dir)
    {
        Vector3 movement = dir * movementSpeed * Time.deltaTime;
        rb.MovePosition(rb.position + movement);

    }

    public void MoveTo(Vector3 pos)
    {
        Vector3 movement = Vector3.MoveTowards(transform.position, pos, movementSpeed * Time.deltaTime);
        rb.MovePosition(movement);
    }

    /// <summary>
    /// Turns the character towards a direction
    /// </summary>
    /// <param name="dir">Direction that the character will turn towards</param>
    public void Turn(Vector3 dir)
    {
        if(dir != Vector3.zero)
        {
            Quaternion turn = Quaternion.LookRotation(dir.normalized);

            rb.MoveRotation(Quaternion.Lerp(rb.rotation, turn, Time.deltaTime * turnSpeed));
        }
    }
}
