using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(CharacterMovement))]
public class PlayerController : MonoBehaviour
{
    [Tooltip("Velocidad de movimiento del jugador")]
    public float movementSpeed = 6f;
    [Tooltip("Velocidad de rotacion del jugador")]
    public float turnSpeed = 20f;

    //Componentes necesarios para el jugador
    private CharacterMovement movementController;
    private Animator anim;
    //Input proporcionado por el PlayerInput
    private Vector3 moveInput;

    // Start is called before the first frame update
    void Start()
    {
        movementController = GetComponent<CharacterMovement>();
        anim = GetComponent<Animator>();
    }

    private void FixedUpdate()
    {
        Move();
        Turn();
    }

    public void SetInput(float _horizontal, float _vertical)
    {
        moveInput.Set(_horizontal, 0f, _vertical);
    }

    private void Move()
    {
        movementController.Move(moveInput.normalized, movementSpeed);
        anim.SetFloat("Speed", moveInput.magnitude);
    }

    private void Turn()
    {
        movementController.Turn(moveInput.normalized, turnSpeed);
    }
}
