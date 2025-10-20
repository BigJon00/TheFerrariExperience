using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;


public class PlayerController : MonoBehaviour
{
    public float moveSpeed = 5f;
    public float jumpForce = 7f;

    private Rigidbody rb;
    private Vector2 moveInput;
    private bool isGrounded;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    void Update()
    {
        isGrounded = Physics.Raycast(transform.position, Vector3.down, 1.1f);
    }

    void FixedUpdate()
    {
        Vector3 movement = new Vector3(moveInput.x, 0, moveInput.y) * moveSpeed;
        rb.velocity = new Vector3(movement.x, rb.velocity.y, movement.z);
    }

    void OnMove(InputValue value)
    {
        moveInput = value.Get<Vector2>();
    }

    void OnJump(InputValue value)
    {
        if (value.isPressed && isGrounded)
        {
            rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
        }
    }
}