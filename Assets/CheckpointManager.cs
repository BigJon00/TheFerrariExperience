using UnityEngine;

public class CheckpointManager : MonoBehaviour
{
    public static CheckpointManager instance;

    private Vector3 savedPosition;
    private Quaternion savedRotation;
    private bool hasCheckpoint = false;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void SetCheckpoint(Vector3 pos, Quaternion rot)
    {
        savedPosition = pos;
        savedRotation = rot;
        hasCheckpoint = true;
        Debug.Log("Checkpoint saved at: " + savedPosition);
    }

    public void RespawnCar(Transform car)
    {
        if (!hasCheckpoint)
        {
            Debug.Log("No checkpoint set yet!");
            return;
        }

        car.position = savedPosition;
        car.rotation = savedRotation;

        Rigidbody rb = car.GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.velocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
        }
    }
}

