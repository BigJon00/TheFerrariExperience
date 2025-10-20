using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class CarController : MonoBehaviour
{
    public float accelerationForce = 30f;
    public float reverseForce = 20f;
    public float turnSpeed = 2f;
    public float brakeForce = 40f;
    public bool invertForwardDirection = false;

    private Rigidbody rb;
    private float verticalInput;
    private float horizontalInput;
    private bool isBraking;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        // Lower center of mass for car-like stability
        rb.centerOfMass = new Vector3(0, -0.5f, 0);
    }

    void FixedUpdate()
    {
        Vector3 forwardDirection = invertForwardDirection ? -transform.forward : transform.forward;

        // movement
        if (verticalInput > 0)
        {
            // Forward
            rb.AddForce(forwardDirection * verticalInput * accelerationForce, ForceMode.Acceleration);
        }
        else if (verticalInput < 0)
        {
            // Reverse
            rb.AddForce(forwardDirection * verticalInput * reverseForce, ForceMode.Acceleration);
        }

        // steering (only effective when moving)
        if (Mathf.Abs(rb.velocity.magnitude) > 0.5f)
        {
            float turn = horizontalInput * turnSpeed * (rb.velocity.magnitude / 10f);
            rb.MoveRotation(rb.rotation * Quaternion.Euler(0, turn, 0));
        }

        // braking
        if (isBraking)
        {
            rb.AddForce(-rb.velocity.normalized * brakeForce, ForceMode.Acceleration);
        }
    }

    void OnMove(InputValue value)
    {
        Vector2 input = value.Get<Vector2>();
        horizontalInput = input.x; // A/D for steering
        verticalInput = input.y;   // W/S for acceleration
    }

    void OnJump(InputValue value)
    {
        isBraking = value.isPressed;
    }
}
