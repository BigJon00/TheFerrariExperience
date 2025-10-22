using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class CarController : MonoBehaviour
{
    public float accelerationForce = 30f;
    public float reverseForce = 20f;
    public float turnSpeed = 2f;
    public float brakeForce = 1f;
    public bool invertForwardDirection = false;

    private Rigidbody rb;
    private float verticalInput;
    private float horizontalInput;
    private bool isBraking;

    public AudioSource engineAudio;
    public AudioSource honkAudio;
    public float minPitch = 0.7f;
    public float maxPitch = 1.5f;
    public float pitchMultiplier = 0.5f;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.centerOfMass = new Vector3(0, -0.5f, 0); // lower center of mass
    }

    void FixedUpdate()
    {
        Vector3 forwardDirection = invertForwardDirection ? -transform.forward : transform.forward;
        float forwardVelocity = Vector3.Dot(rb.velocity, forwardDirection);
        bool isMovingForward = forwardVelocity > 0.5f;
        bool isMovingBackward = forwardVelocity < -0.5f;

        // movement
        if (verticalInput > 0)
        {
            // Forward
            rb.AddForce(forwardDirection * verticalInput * accelerationForce, ForceMode.Acceleration);
        } else if (verticalInput < 0)
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

        HandleEngineAudio();
    }

    void OnMove(InputValue value)
    {
        Vector2 input = value.Get<Vector2>();
        horizontalInput = input.x; // A/D for steering
        verticalInput = input.y;   // W/S for acceleration
    }

    void OnJump(InputValue value)
    {
        if (value.isPressed && honkAudio != null)
        {
            honkAudio.Play();
        }
    }

    void HandleEngineAudio()
    {
        if (engineAudio == null) return;

        Vector3 forwardDirection = invertForwardDirection ? -transform.forward : transform.forward;
        float forwardVelocity = Vector3.Dot(rb.velocity, forwardDirection);

        float currentSpeed = Mathf.Abs(forwardVelocity);

        // Only play engine sound when:
        // 1. We're pressing forward AND actually moving forward, OR
        // 2. We're pressing reverse AND actually moving backward
        bool shouldPlayEngine =
            (verticalInput > 0 && forwardVelocity > -1f) ||  // Pressing forward and not moving backward fast
            (verticalInput < 0 && forwardVelocity < 1f);     // Pressing reverse and not moving forward fast

        // Calculate target pitch based on speed
        float targetPitch = minPitch + (currentSpeed * pitchMultiplier);
        targetPitch = Mathf.Clamp(targetPitch, minPitch, maxPitch);

        // Handle engine audio playback
        if (shouldPlayEngine)
        {
            if (!engineAudio.isPlaying)
                engineAudio.Play();

            // Smoothly change pitch
            engineAudio.pitch = Mathf.Lerp(engineAudio.pitch, targetPitch, Time.deltaTime * 2f);
        }
        else
        {
            // Fade out engine sound when not appropriate
            if (engineAudio.isPlaying)
            {
                engineAudio.pitch = Mathf.Lerp(engineAudio.pitch, minPitch, Time.deltaTime * 4f);

                // Stop if pitch is very close to minimum
                if (Mathf.Abs(engineAudio.pitch - minPitch) < 0.1f)
                {
                    engineAudio.Stop();
                }
            }
        }
    }
}
