using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(PlayerController))]
public class PlayerInput : MonoBehaviour
{
    private PlayerController _controller;

    public PlayerController controller { get { return _controller; } }
    // Start is called before the first frame update
    void Start()
    {
        _controller = GetComponent<PlayerController>();
    }

    // Update is called once per frame
    void Update()
    {
        _controller.SetInput(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));
    }
}
