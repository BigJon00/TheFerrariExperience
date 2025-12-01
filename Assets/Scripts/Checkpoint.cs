using UnityEngine;

public class Checkpoint : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            CheckpointManager.instance.SetCheckpoint(
                other.transform.position,
                other.transform.rotation
            );
        }
    }
}
