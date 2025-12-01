using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using TMPro;

public class CarController : MonoBehaviour
{
    // car settings
    public float accelerationForce = 30f;
    public float reverseForce = 20f;
    public float turnSpeed = 2f;
    public float brakeForce = 1f;
    public bool invertForwardDirection = false;

    // rain settings
    public float slipperyTurnMultiplier = 0.6f; // 40% less turning when slippery
    public float slipperyDrag = 0.005f; // Less drag = more sliding
    public float normalDrag = 0.1f;
    public float slipperyAccelerationMultiplier = 0.8f; // Reduce acceleration

    private Rigidbody rb;
    private float verticalInput;
    private float horizontalInput;
    private bool isBraking;
    private Events eventsManager;
    private float currentTurnSpeed;
    private float currentAccelerationForce;
    private float currentBrakeForce;

    public AudioSource engineAudio;
    public AudioSource honkAudio;
    public float minPitch = 0.7f;
    public float maxPitch = 1.5f;
    public float pitchMultiplier = 0.5f;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.centerOfMass = new Vector3(0, -0.5f, 0); // lower center of mass
        currentTurnSpeed = turnSpeed;
        currentAccelerationForce = accelerationForce;
        eventsManager = FindObjectOfType<Events>();
    }

    void FixedUpdate()
    {
        Vector3 forwardDirection = invertForwardDirection ? -transform.forward : transform.forward;
        float forwardVelocity = Vector3.Dot(rb.velocity, forwardDirection);
        bool isMovingForward = forwardVelocity > 0.5f;
        bool isMovingBackward = forwardVelocity < -0.5f;

        ApplySlipperyPhysics();

        // movement
        if (verticalInput > 0)
        {
            // Forward
            rb.AddForce(forwardDirection * verticalInput * currentAccelerationForce, ForceMode.Acceleration);
        }
        else if (verticalInput < 0)
        {
            // Reverse
            rb.AddForce(forwardDirection * verticalInput * reverseForce * (currentAccelerationForce / accelerationForce), ForceMode.Acceleration);
        }

        // steering (only effective when moving)
        if (Mathf.Abs(rb.velocity.magnitude) > 0.5f)
        {
            float turn = horizontalInput * currentTurnSpeed * (rb.velocity.magnitude / 10f);
            if (verticalInput < 0 || isMovingBackward)
            {
                turn = -turn;
            }
            rb.MoveRotation(rb.rotation * Quaternion.Euler(0, turn, 0));
        }

        // braking
        if (isBraking)
        {
            rb.AddForce(-rb.velocity.normalized * currentBrakeForce, ForceMode.Acceleration);
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
                void Update()
{
    if (Keyboard.current.rKey.wasPressedThisFrame)
    {
        CheckpointManager.instance.RespawnCar(transform);
    }
}

            }
        }
    }

    void ApplySlipperyPhysics()
    {
        bool isSlippery = eventsManager != null && eventsManager.isTrackSlippery;

        if (isSlippery)
        {
            // EXTREME drag reduction for maximum sliding
            rb.drag = slipperyDrag; // Almost no drag - car will slide forever
            rb.angularDrag = 0.05f; // Very low angular drag for spinning

            // Reduce turning responsiveness
            currentTurnSpeed = turnSpeed * slipperyTurnMultiplier; // 80% less turning!

            // Reduce acceleration
            currentAccelerationForce = accelerationForce * slipperyAccelerationMultiplier;

            // Make brakes almost useless
            currentBrakeForce = brakeForce * 0.1f;

            // Add continuous sideways drift when turning
            if (Mathf.Abs(horizontalInput) > 0.1f && rb.velocity.magnitude > 3f)
            {
                // Strong sideways force that makes the car drift
                Vector3 driftForce = transform.right * horizontalInput * 5f * (rb.velocity.magnitude / 10f);
                rb.AddForce(driftForce, ForceMode.Acceleration);
            }

            // Add random sliding forces more frequently
            if (rb.velocity.magnitude > 5f)
            {
                // Frequent random forces that make the car feel like it's on ice
                Vector3 randomForce = new Vector3(
                    Random.Range(-1f, 1f) * (rb.velocity.magnitude / 8f),
                    0,
                    Random.Range(-1f, 1f) * (rb.velocity.magnitude / 8f)
                );
                rb.AddForce(randomForce, ForceMode.Impulse);

                // More frequent spinning
                if (Random.Range(0f, 1f) > 0.5f) // 50% chance per frame when moving
                {
                    Vector3 spinTorque = new Vector3(
                        0,
                        Random.Range(-3f, 3f) * Mathf.Sign(horizontalInput) * (rb.velocity.magnitude / 10f),
                        0
                    );
                    rb.AddTorque(spinTorque, ForceMode.Impulse);
                }
            }

            // Reduce grip when braking
            if (isBraking)
            {
                // Instead of braking, add more sliding
                Vector3 brakeSlideForce = new Vector3(
                    Random.Range(-2f, 2f),
                    0,
                    Random.Range(-2f, 2f)
                );
                rb.AddForce(brakeSlideForce, ForceMode.Impulse);
            }
        }
        else
        {
            // Normal physics
            currentTurnSpeed = turnSpeed;
            currentAccelerationForce = accelerationForce;
            currentBrakeForce = brakeForce;
            rb.drag = normalDrag;
            rb.angularDrag = 0.5f;
        }
    }

    void OnGUI()
    {
        if (eventsManager != null && eventsManager.isTrackSlippery)
        {
            GUI.color = Color.yellow;

            // Calculate position for bottom center
            float boxWidth = 300f;
            float boxHeight = 40f;
            float xPos = (Screen.width - boxWidth) / 2f; // Center horizontally
            float yPos = Screen.height - boxHeight - 20f; // 20 pixels from bottom

            GUI.Box(new Rect(xPos, yPos, boxWidth, boxHeight), "SLIPPERY ROAD - REDUCED TRACTION!");
        }
    }

    // getter methods
    public float CurrentSpeed
    {
        get { return rb.velocity.magnitude * 3.6f; }
    }
}