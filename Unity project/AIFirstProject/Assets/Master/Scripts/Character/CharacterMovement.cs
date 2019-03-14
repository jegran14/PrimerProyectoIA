using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class CharacterMovement : MonoBehaviour
{
    public float movementSpeed = 6f;
    public float turnSpeed = 20f;

    private Rigidbody rb;
    private Animator anim;
    private Vector3 moveInput;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        anim = GetComponent<Animator>();
        moveInput = Vector3.zero;
    }

    private void FixedUpdate()
    {
        Move();
        Turn();
    }

    /// <summary>
    /// This function will receive the vertival and horizontal inputs necessary to move the character
    /// </summary>
    /// <param name="h">Input necessary for the movement for the horizontal axis, or 'X' axis</param>
    /// <param name="v">Input necessary for the movement for the vertical axis, or 'Z' axis</param>
    public void SetInput(float _horizontal, float _vertical)
    {
        moveInput.Set(_horizontal, 0f, _vertical);
    }

    private void Move()
    {
        Vector3 movement = moveInput.normalized * movementSpeed * Time.deltaTime;
        rb.MovePosition(rb.position + movement);

    }

    /// <summary>
    /// The character turns towards the movement direction (Make an override from this class if we want to change that)
    /// </summary>
    private void Turn()
    {
        if(moveInput != Vector3.zero)
        {
            Quaternion turn = Quaternion.LookRotation(moveInput.normalized);

            rb.MoveRotation(Quaternion.Lerp(rb.rotation, turn, Time.deltaTime * turnSpeed));
        }

        anim.SetFloat("Speed", moveInput.magnitude);
    }
}
