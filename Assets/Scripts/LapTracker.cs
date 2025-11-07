using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class LapTracker : MonoBehaviour
{
    public TextMeshProUGUI lapText;
    public int lap = 1;
    public int enemyLaps = 1;
    private Events eventsManager;

    void Start()
    {
        eventsManager = FindObjectOfType<Events>();
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            lap = lap + 1;
            SetLapText();
        } else if (other.gameObject.CompareTag("Enemy")) {
            enemyLaps = enemyLaps + 1;
        }
    }

    void SetLapText()
    {
        lapText.text = "Lap: " + lap.ToString() + "/6";
        if (lap >= 6 && eventsManager != null)
        {
            eventsManager.ShowWinScreen();
        }
    }
}
