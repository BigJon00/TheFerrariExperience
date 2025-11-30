using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class LeaderboardController : MonoBehaviour
{
    public GameObject leaderboardEntryPrefab;
    public Transform leaderboardContent;
    public TextMeshProUGUI playerPositionText;

    void Start()
    {
        DisplayLeaderboard();
    }

    void DisplayLeaderboard()
    {
        if (RaceResults.FinalResults == null || RaceResults.FinalResults.Count == 0)
        {
            Debug.LogWarning("No race results found!");
            return;
        }

        // Clear existing entries
        foreach (Transform child in leaderboardContent)
        {
            Destroy(child.gameObject);
        }

        // Create leaderboard entries
        for (int i = 0; i < RaceResults.FinalResults.Count; i++)
        {
            var result = RaceResults.FinalResults[i];
            result.Position = i + 1;

            GameObject entry = Instantiate(leaderboardEntryPrefab, leaderboardContent);
            TextMeshProUGUI entryText = entry.GetComponent<TextMeshProUGUI>();

            if (entryText != null)
            {
                entryText.text = $"{result.Position}. {result.Name} - Lap {result.LapCount}/6";

                // Highlight player entry
                if (result.Name == "Ferrari")
                {
                    entryText.color = Color.yellow;
                    playerPositionText.text = $"You finished {GetPositionSuffix(result.Position)}!";
                }
            }
        }

        // Clean up the LapTracker instance
        if (LapTracker.Instance != null)
        {
            Destroy(LapTracker.Instance.gameObject);
        }
    }

    string GetPositionSuffix(int position)
    {
        switch (position)
        {
            case 1: return "1st";
            case 2: return "2nd";
            case 3: return "3rd";
            default: return $"{position}th";
        }
    }
}
