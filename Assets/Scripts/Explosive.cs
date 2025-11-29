using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Explosive : MonoBehaviour
{
    // explosive settings
    public float explosionPower = 30f;
    public float explosionSize = 8f;
    public float upwardForce = 2f;
    public AudioSource explosionSound;
    public GameObject explosionEffect;

    void OnCollisionEnter(Collision collision)
    {
        // explosion effects
        if (explosionEffect != null)
        {
            GameObject effectInstance = Instantiate(explosionEffect, transform.position, Quaternion.identity);
            Destroy(effectInstance, 3f);
        }

        // explosion sound
        if (explosionSound != null)
        {
            explosionSound.Play();
        }

        // knockback
        //if (collision.gameObject.CompareTag("Player") || collision.gameObject.CompareTag("Enemy")) 
        //{
            Collider[] colliders = Physics.OverlapSphere(transform.position, explosionSize);

            foreach (Collider hit in colliders)
            {
                if (hit.CompareTag("Player") || hit.CompareTag("Enemy"))
                {
                    Rigidbody carRb = hit.GetComponent<Rigidbody>();
                    if (carRb != null)
                    {
                        carRb.AddExplosionForce(explosionPower, transform.position, explosionSize, upwardForce, ForceMode.Impulse);
                    }
                }
            }
            Destroy(gameObject);
        //}
    }
}
