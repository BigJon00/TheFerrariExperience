using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Events : MonoBehaviour
{ 
    public float fadeDuration = 1f;
    public float displayImageDuration = 1f;

    public bool blizzardEvent = false;
    public CanvasGroup blizzardCanvasGroup;
    public CanvasGroup frostedScreen;
    public float frostedScreenDuration = 5f;
    public AudioSource blizzardAudio;

    public bool meteorEvent = false;
    public CanvasGroup meteorCanvasGroup;
    public AudioSource meteorAudio;

    private bool isEventRunning = false;

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

    IEnumerator RunBlizzardEvent()
    {
        isEventRunning = true;

        if (!blizzardAudio.isPlaying)
        {
            blizzardAudio.Play();
        }

        yield return StartCoroutine(FadeInAndOut(blizzardCanvasGroup));

        yield return StartCoroutine(FadeFrostedScreen());

        blizzardEvent = false;
        isEventRunning = false;
    }

    IEnumerator RunMeteorEvent()
    {
        isEventRunning = true;

        if (!meteorAudio.isPlaying)
        {
            meteorAudio.Play();
        }

        yield return StartCoroutine(FadeInAndOut(meteorCanvasGroup));
        meteorEvent = false;
        isEventRunning = false;
    }

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
}
