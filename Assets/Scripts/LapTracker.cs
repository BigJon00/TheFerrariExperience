using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class LapTracker : MonoBehaviour
{
    public TextMeshProUGUI lapText;
    public int lap = 1;

    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            lap = lap + 1;
            SetLapText();
        }
    }
    void SetLapText()
    {
        lapText.text = "Lap: " + lap.ToString() + "/6";
        //if (lap >= 6)
        //{
        //    LoadEndScene();
        //}

    }
}
