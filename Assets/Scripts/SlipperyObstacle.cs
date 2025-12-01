using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SlipperyObstacle : MonoBehaviour
{
    public float spinForce = 10f;
    public float slipDuration = 3f;

    public AudioSource slipAudio;

    void Start()
    {
        //Debug.Log($"SlipperyObstacle started on: {gameObject.name}");
    }

    private void OnTriggerEnter(Collider other)
    {
        //Debug.Log($"SlipperyObstacle triggered by: {other.gameObject.name}");
        if (other.CompareTag("Player") || other.CompareTag("Enemy"))
        {
            Rigidbody rb = other.GetComponent<Rigidbody>();
            if (rb != null)
            {
                if (slipAudio != null)
                {
                    slipAudio.Play();
                }
                //Debug.Log("Applying spin force to car");
                ApplySpin(rb);
            }
        }
    }

    private void ApplySpin(Rigidbody rb)
    {
        // Apply stronger, more dramatic spin
        Vector3 randomSpin = new Vector3(
            Random.Range(-1f, 1f),
            Random.Range(0.5f, 1f), // Add some upward spin
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
        //Debug.Log($"Applied spin force: {randomSpin}");
    }
}
