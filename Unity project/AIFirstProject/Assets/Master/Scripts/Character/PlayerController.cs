using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(CharacterMovement))]
public class PlayerController : MonoBehaviour
{
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
        movementController.Move(moveInput.normalized);
        anim.SetFloat("Speed", moveInput.magnitude);
    }

    private void Turn()
    {
        movementController.Turn(moveInput.normalized);
    }
}
