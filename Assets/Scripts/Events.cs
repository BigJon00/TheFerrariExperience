using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Events : MonoBehaviour
{
    public float fadeDuration = 1f;
    public float displayImageDuration = 1f;

    // Event settings
    public bool eventsEnabled = true;
    public float minTimeBetweenEvents = 10f;
    public float maxTimeBetweenEvents = 30f;

    // Event probabilities
    [Range(0, 100)] public float blizzardChance = 50f;
    [Range(0, 100)] public float meteorChance = 50f;

    // Event references; ~15 seconds each
    public bool blizzardEvent = false;    
    public CanvasGroup blizzardCanvasGroup;
    public AudioSource blizzardAudio;
    public CanvasGroup frostedScreen;
    public float frostedScreenDuration = 5f;

    public bool meteorEvent = false;
    public CanvasGroup meteorCanvasGroup;
    public AudioSource meteorAudio;
    public GameObject meteorPrefab;
    public float meteorSpawnDuration = 15f;
    public float minMeteorSpawnRate = 0.5f;
    public float maxMeteorSpawnRate = 2f;
    public Vector3 spawnAreaCenter = new Vector3(0, 30, -150);
    public Vector3 spawnAreaSize = new Vector3(300, 10, 150);
    public float trackMargin = 2f;

    public CanvasGroup winScreenCanvasGroup;

    private bool isEventRunning = false;
    private bool eventSchedulerRunning = false;
    private Bounds combinedTrackBounds;

    void Start()
    {
        HideBlizzardObjects();

        CalculateCombinedTrackBounds();

        if (eventsEnabled && !eventSchedulerRunning)
        {
            StartCoroutine(EventScheduler());
        }
    }

    void Update()
    {
        if (blizzardEvent && !isEventRunning) // activate blizzard event
        {
            StartCoroutine(RunBlizzardEvent());
        }
        if (meteorEvent && !isEventRunning) // activate meteor shower event
        {
            StartCoroutine(RunMeteorEvent());
        }
    }

    /////////////////////////////////////////////////////////////////

    IEnumerator EventScheduler()
    {
        eventSchedulerRunning = true;
        while (eventsEnabled)
        {
            float waitTime = Random.Range(minTimeBetweenEvents, maxTimeBetweenEvents);
            yield return new WaitForSeconds(waitTime);
            if (!isEventRunning && eventsEnabled)
            {
                TriggerRandomEvent();
            }
        }
        eventSchedulerRunning = false;
    }

    void TriggerRandomEvent()
    {
        float totalChance = blizzardChance + meteorChance;
        if (totalChance <= 0)
        {
            return;
        }

        float randomValue = Random.Range(0f, totalChance);
        if (randomValue <= blizzardChance)
        {
            blizzardEvent = true;
        }
        else
        {
            meteorEvent = true;
        }

    }

    // Blizzard Event //
    IEnumerator RunBlizzardEvent()
    {
        isEventRunning = true;

        if (!blizzardAudio.isPlaying)
        {
            blizzardAudio.Play();
        }

        ShowBlizzardObjects();
        yield return StartCoroutine(FadeInAndOut(blizzardCanvasGroup));

        yield return StartCoroutine(FadeFrostedScreen());

        HideBlizzardObjects();
        Debug.Log("Blizzard event over");
        blizzardEvent = false;
        isEventRunning = false;
    }

    IEnumerator FadeFrostedScreen()
    {
        // Fade in frosted screen
        float timer = 0f;
        while (timer < fadeDuration)
        {
            timer += Time.deltaTime;
            frostedScreen.alpha = Mathf.Lerp(0f, 0.5f, timer / fadeDuration);
            yield return null;
        }
        frostedScreen.alpha = 0.5f;

        // Keep frosted screen visible for a while
        yield return new WaitForSeconds(frostedScreenDuration);

        // Fade out frosted screen
        timer = 0f;
        while (timer < fadeDuration)
        {
            timer += Time.deltaTime;
            frostedScreen.alpha = Mathf.Lerp(0.5f, 0f, timer / fadeDuration);
            yield return null;
        }
        frostedScreen.alpha = 0f;
    }

    void ShowBlizzardObjects() // activate blizzard objects
    {
        GameObject[] allObjects = Resources.FindObjectsOfTypeAll<GameObject>();
        int activatedCount = 0;

        foreach (GameObject obj in allObjects)
        {
            if (obj.CompareTag("Blizzard") && !obj.activeInHierarchy)
            {
                obj.SetActive(true);
                activatedCount++;
            }
        }
        Debug.Log($"Activated {activatedCount} blizzard objects");
    }

    void HideBlizzardObjects() // deactivate blizzard objects
    {
        GameObject[] blizzardObjects = GameObject.FindGameObjectsWithTag("Blizzard");
        foreach (GameObject obj in blizzardObjects)
        {
            obj.SetActive(false);
        }
        Debug.Log($"Hidden {blizzardObjects.Length} blizzard objects");
    }

    // Meteor Event //
    IEnumerator RunMeteorEvent()
    {
        isEventRunning = true;

        if (!meteorAudio.isPlaying)
        {
            meteorAudio.Play();
        }

        StartCoroutine(SpawnMeteors());

        StartCoroutine(FadeInAndOut(meteorCanvasGroup));

        yield return new WaitForSeconds(meteorSpawnDuration);

        Debug.Log("Meteor shower event over");
        meteorEvent = false;
        isEventRunning = false;
    }

    IEnumerator SpawnMeteors()
    {
        float eventEndTime = Time.time + meteorSpawnDuration;
        while (Time.time < eventEndTime && meteorEvent)
        {
            float currentSpawnRate = Random.Range(minMeteorSpawnRate, maxMeteorSpawnRate);
            float spawnDelay = 1f / currentSpawnRate;
            SpawnSingleMeteor();
            yield return new WaitForSeconds(spawnDelay);
        }
    }

    void SpawnSingleMeteor()
    {
        if (meteorPrefab == null)
        {
            Debug.LogWarning("Meteor prefab not assigned");
            return;
        }

        Vector3 spawnPosition = GetTrackSpawnPosition();

        GameObject meteor = Instantiate(meteorPrefab, spawnPosition, Random.rotation);
        //Debug.Log("Spawned meteor at: " + spawnPosition);
        Debug.Log($"Spawned meteor at: {spawnPosition} (Track bounds: {combinedTrackBounds})");

    }

    Vector3 GetTrackSpawnPosition()
    {
        if (combinedTrackBounds.size != Vector3.zero)
        {
            return new Vector3(
                Random.Range(combinedTrackBounds.min.x - trackMargin, combinedTrackBounds.max.x + trackMargin),
                spawnAreaCenter.y,
                Random.Range(combinedTrackBounds.min.z - trackMargin, combinedTrackBounds.max.z + trackMargin)
            );
        }
        Debug.LogWarning("No track objects found, using fallback spawn");
        return GetRandomSpawnPosition();
    }

    Vector3 GetRandomSpawnPosition()
    {
        return new Vector3(
            Random.Range(spawnAreaCenter.x - spawnAreaSize.x / 2, spawnAreaCenter.x + spawnAreaSize.x / 2),
            Random.Range(spawnAreaCenter.y - spawnAreaSize.y / 2, spawnAreaCenter.y + spawnAreaSize.y / 2),
            Random.Range(spawnAreaCenter.z - spawnAreaSize.z / 2, spawnAreaCenter.z + spawnAreaSize.z / 2));
    }

    void CalculateCombinedTrackBounds()
    {
        GameObject[] allTracks = GameObject.FindGameObjectsWithTag("Track");

        if (allTracks.Length == 0)
        {
            Debug.LogWarning("No objects found with tag 'Track'");
            return;
        }

        // Start with first track's bounds
        Renderer firstRenderer = allTracks[0].GetComponent<Renderer>();
        if (firstRenderer != null)
        {
            combinedTrackBounds = firstRenderer.bounds;

            // Expand bounds to include all other tracks
            for (int i = 1; i < allTracks.Length; i++)
            {
                Renderer trackRenderer = allTracks[i].GetComponent<Renderer>();
                if (trackRenderer != null)
                {
                    combinedTrackBounds.Encapsulate(trackRenderer.bounds);
                }
            }

            Debug.Log($"Combined track bounds: {combinedTrackBounds}");
        }
    }

    // General Events //
    IEnumerator FadeInAndOut(CanvasGroup imageCanvasGroup)
    {
        // Fade in
        float timer = 0f;
        while (timer < fadeDuration)
        {
            timer += Time.deltaTime;
            imageCanvasGroup.alpha = timer / fadeDuration;
            yield return null;
        }
        imageCanvasGroup.alpha = 1f;

        // Wait for 1 second
        yield return new WaitForSeconds(displayImageDuration);

        // Fade out
        timer = 0f;
        while (timer < fadeDuration)
        {
            timer += Time.deltaTime;
            imageCanvasGroup.alpha = 1 - (timer / fadeDuration);
            yield return null;
        }
        imageCanvasGroup.alpha = 0f;
    }

    public void ShowWinScreen()
    {
        StartCoroutine(FadeInAndOut(winScreenCanvasGroup));
    }
}
