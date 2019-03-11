using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(CharacterMovement))]
public class PlayerInput : MonoBehaviour
{
    private CharacterMovement movement;
    // Start is called before the first frame update
    void Start()
    {
        movement = GetComponent<CharacterMovement>();
    }

    // Update is called once per frame
    void Update()
    {
        movement.SetInput(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));
    }
}
