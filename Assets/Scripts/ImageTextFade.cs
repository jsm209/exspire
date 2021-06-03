using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ImageTextFade : MonoBehaviour
{
    public CanvasGroup images;
    public CanvasGroup texts;

    public float imageDisplayDuration;
    public float imageFadeDuration;

    public float textDisplayDuration;
    public float textFadeDuration;

    public bool pressSpaceToContinue;

    void Start()
    {
        // Make all the text invisible
        texts.alpha = 0;

        if (!pressSpaceToContinue)
        {
            StartCoroutine(FadeInThenOut(texts, textFadeDuration, textDisplayDuration));
            StartCoroutine(FadeOutAfterDelay(images, imageFadeDuration, imageDisplayDuration)); 
        } else {
            StartCoroutine(FadeCanvas(texts, 0f, 1f, textFadeDuration));
        }

    }

    void Update() {
        if (pressSpaceToContinue && Input.GetKeyDown(KeyCode.Space)) {
            StartCoroutine(FadeCanvas(texts, 1f, 0f, textFadeDuration));
            StartCoroutine(FadeCanvas(images, 1f, 0f, imageFadeDuration));
        }
    }

    IEnumerator FadeInThenOut(CanvasGroup canvas, float fadeDuration, float displayDuration)
    {
        StartCoroutine(FadeCanvas(canvas, 0f, 1f, fadeDuration));
        yield return new WaitForSeconds(displayDuration);
        StartCoroutine(FadeCanvas(canvas, 1f, 0f, fadeDuration));
    }

    IEnumerator FadeOutAfterDelay(CanvasGroup canvas, float fadeDuration, float displayDuration)
    {
        yield return new WaitForSeconds(displayDuration);
        StartCoroutine(FadeCanvas(canvas, 1f, 0f, fadeDuration));
    }

    IEnumerator FadeCanvas(CanvasGroup canvas, float startAlpha, float endAlpha, float duration)
    {
        // keep track of when the fading started, when it should finish, and how long it has been running&lt;/p&gt; &lt;p&gt;&a
        var startTime = Time.time;
        var endTime = Time.time + duration;
        var elapsedTime = 0f;

        // set the canvas to the start alpha – this ensures that the canvas is ‘reset’ if you fade it multiple times
        canvas.alpha = startAlpha;
        // loop repeatedly until the previously calculated end time
        while (Time.time <= endTime)
        {
            elapsedTime = Time.time - startTime; // update the elapsed time
            var percentage = 1 / (duration / elapsedTime); // calculate how far along the timeline we are
            if (startAlpha > endAlpha) // if we are fading out/down 
            {
                canvas.alpha = startAlpha - percentage; // calculate the new alpha
            }
            else // if we are fading in/up
            {
                canvas.alpha = startAlpha + percentage; // calculate the new alpha
            }

            yield return new WaitForEndOfFrame(); // wait for the next frame before continuing the loop
        }
        canvas.alpha = endAlpha; // force the alpha to the end alpha before finishing – this is here to mitigate any rounding errors, e.g. leaving the alpha at 0.01 instead of 0
    }
}
