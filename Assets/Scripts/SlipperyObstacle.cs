using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SlipperyObstacle : MonoBehaviour
{
    public float spinForce = 10f;
    public float slipDuration = 3f;

    public AudioSource slipAudio;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Rigidbody rb = other.GetComponent<Rigidbody>();
            if (rb != null)
            {
                if (slipAudio != null)
                {
                    slipAudio.Play();
                }
                ApplySpin(rb);
            }
        }
    }

    private void ApplySpin(Rigidbody rb)
    {
        // Apply stronger, more dramatic spin
        Vector3 randomSpin = new Vector3(
            Random.Range(-1f, 1f),
            Random.Range(0.5f, 1f), // Add some upward spin for more drama
            Random.Range(-1f, 1f)
        ).normalized * spinForce;

        rb.AddTorque(randomSpin, ForceMode.VelocityChange);

        // Also add a strong sideways force
        Vector3 sidewaysForce = new Vector3(
            Random.Range(-1f, 1f),
            0,
            Random.Range(-1f, 1f)
        ).normalized * spinForce * 0.8f;

        rb.AddForce(sidewaysForce, ForceMode.VelocityChange);
    }
}
