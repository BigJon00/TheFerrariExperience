using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class SpeedBoost : MonoBehaviour
{
    // speed boost settings
    public float speedBoostMultiplier = 1.5f;
    public float boostDuration = 3f;
    public AudioSource boostAudio;

    public void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            CarController playerCar = other.GetComponent<CarController>();
            if (playerCar != null)
            {
                ApplySpeedBoost(playerCar);
            }
        }
        else if (other.CompareTag("Enemy"))
        {
            // ai collision code here
        }

        if (boostAudio != null)
        {
            boostAudio.Play();
        }
        Destroy(gameObject);
    }

    private void ApplySpeedBoost(CarController carController)
    {
        StartCoroutine(SpeedBoostCoroutine(carController));
        Debug.Log("Speed boost applied");
    }

    private System.Collections.IEnumerator SpeedBoostCoroutine(CarController carController)
    {
        float originalAcceleration = carController.accelerationForce;
        carController.accelerationForce *= speedBoostMultiplier;
        yield return new WaitForSeconds(boostDuration);
        carController.accelerationForce = originalAcceleration;
        Debug.Log("Speed boost ended");
    }
}
