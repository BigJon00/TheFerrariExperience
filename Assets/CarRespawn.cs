using UnityEngine;
using UnityEngine.InputSystem;

public class CarRespawn : MonoBehaviour
{
    void Update()
    {
        if (Keyboard.current.rKey.wasPressedThisFrame)
        {
            Debug.Log("R pressed — trying to respawn");
            CheckpointManager.instance.RespawnCar(transform);
        }
    }
}
