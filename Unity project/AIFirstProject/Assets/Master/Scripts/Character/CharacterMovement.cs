using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class CharacterMovement : MonoBehaviour
{
    private Rigidbody rb;
    private LevelChanger sceneScript;
    private Animator animator;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        animator = GetComponent<Animator>();
    }

    /// <summary>
    /// Moves the character in the desired direction
    /// </summary>
    /// <param name="dir">Movement direction</param>
    /// <param name="movementSpeed">Movement Speed</param>
    public void Move(Vector3 dir, float movementSpeed)
    {
        Vector3 movement = dir * movementSpeed * Time.deltaTime;
        rb.MovePosition(rb.position + movement);

    }

    /// <summary>
    /// Move character to a desired position
    /// </summary>
    /// <param name="pos">Target position to move the character to</param>
    /// <param name="movementSpeed">Movement speed</param>
    public void MoveTo(Vector3 pos, float movementSpeed)
    {
        Vector3 movement = Vector3.MoveTowards(transform.position, pos, movementSpeed * Time.deltaTime);
        rb.MovePosition(movement);
    }

    /// <summary>
    /// Turns the character towards a direction
    /// </summary>
    /// <param name="dir">Direction that the character will turn towards</param>
    /// <param name="turnSpeend">Turning speed</param>
    public void Turn(Vector3 dir, float turnSpeed)
    {
        if(dir != Vector3.zero)
        {
            Quaternion turn = Quaternion.LookRotation(dir.normalized);

            rb.MoveRotation(Quaternion.Lerp(rb.rotation, turn, Time.deltaTime * turnSpeed));
        }
    }
}
