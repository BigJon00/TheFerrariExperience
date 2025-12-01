using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class LapTracker : MonoBehaviour
{
    public TextMeshProUGUI lapText;
    public int playerLap = 0;
    public int totalLaps = 6;

    private Dictionary<string, int> enemyLaps = new Dictionary<string, int>();
    public static LapTracker Instance { get; private set; }

    void Awake()
    {
        // Singleton pattern to persist between scenes
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {
        SetLapText();
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player") || other.gameObject.CompareTag("Enemy"))
        {
            TrackCarLap(other.gameObject);

            // Check if ANY car has finished the race
            CheckRaceCompletion();
        }
    }

    void TrackCarLap(GameObject car)
    {
        string carName = car.name;

        if (car.CompareTag("Player"))
        {
            playerLap++;
            SetLapText();
            Debug.Log($"Player completed lap {playerLap}");
        }
        else if (car.CompareTag("Enemy"))
        {
            if (enemyLaps.ContainsKey(carName))
            {
                enemyLaps[carName]++;
            }
            else
            {
                enemyLaps.Add(carName, 1);
            }
            Debug.Log($"{carName} completed lap {enemyLaps[carName]}");
        }
    }

    void CheckRaceCompletion()
    {
        // Check if player has finished
        if (playerLap >= totalLaps)
        {
            EndRace();
            return;
        }

        // Check if any enemy has finished (they might finish before player)
        //foreach (var enemy in enemyLaps)
        //{
        //    if (enemy.Value >= totalLaps)
        //    {
        //        EndRace();
        //        return;
        //    }
        //}
    }

    void EndRace()
    {
        Debug.Log("Race finished! Calculating results...");
        StoreRaceResults();
        SceneManager.LoadScene("End_Scene");
    }

    void SetLapText()
    {
        lapText.text = "Lap: " + playerLap.ToString() + "/" + totalLaps;
    }

    void StoreRaceResults()
    {
        List<RaceResult> raceResults = new List<RaceResult>();

        // Add player result
        raceResults.Add(new RaceResult("Ferrari", playerLap, true));

        // Add enemy results
        foreach (var enemy in enemyLaps)
        {
            string displayName = GetDisplayName(enemy.Key);
            raceResults.Add(new RaceResult(displayName, enemy.Value, false));
        }

        // Sort by lap count (descending), then by who finished first
        raceResults.Sort((a, b) =>
        {
            // First sort by lap count (higher laps first)
            int lapComparison = b.LapCount.CompareTo(a.LapCount);
            if (lapComparison != 0) return lapComparison;

            // If lap count is equal, player gets priority (you can change this logic)
            if (a.IsPlayer) return -1;
            if (b.IsPlayer) return 1;

            return 0;
        });

        // Assign final positions
        for (int i = 0; i < raceResults.Count; i++)
        {
            raceResults[i].Position = i + 1;
        }

        RaceResults.FinalResults = raceResults;
    }

    string GetDisplayName(string originalName)
    {
        // Map original names to display names
        if (originalName.Contains("Player")) return "Ferrari";
        if (originalName.Contains("McLaren")) return "McLaren";
        if (originalName.Contains("Mercedes")) return "Mercedes";
        if (originalName.Contains("RedBull")) return "Red Bull";
        if (originalName.Contains("Williams")) return "Williams";
        if (originalName.Contains("Alpine")) return "Alpine";
        if (originalName.Contains("RacingBulls")) return "Racing Bulls";
        if (originalName.Contains("AstonMartin")) return "Aston Martin";
        if (originalName.Contains("Haas")) return "Haas";
        if (originalName.Contains("Sauber")) return "Sauber";
        if (originalName.Contains("Enemy_")) return originalName.Replace("Enemy_", "");

        // Default: remove "Enemy_" prefix and any other unwanted text
        return originalName.Replace("Enemy_", "").Replace("(Clone)", "").Trim();
    }
}
// Class to store race result data
[System.Serializable]
public class RaceResult
{
    public string Name;
    public int LapCount;
    public int Position;
    public bool IsPlayer;

    public RaceResult(string name, int lapCount, bool isPlayer)
    {
        Name = name;
        LapCount = lapCount;
        IsPlayer = isPlayer;
    }
}

// Static class to persist data between scenes
public static class RaceResults
{
    public static List<RaceResult> FinalResults { get; set; } = new List<RaceResult>();
}