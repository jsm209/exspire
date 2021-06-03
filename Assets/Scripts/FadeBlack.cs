using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FadeBlack : MonoBehaviour
{
    public CanvasGroup images;

    public bool showBlack; // Should we be seeing black right now?
    public float imageFadeDuration;

    public bool autoToggleSelfOnce; // Should we toggle this automatically once?
    public float autoToggleDelay; // How long should we wait before toggling this?

    private bool transitioning; // Are we currently transitioning right now?
    private bool showBlackPrevState; // Keeps 

    void Start()
    {
        transitioning = false;
        showBlackPrevState = showBlack;

        // If showBlack is initially ticked, we shouldn't do a fade but just instantly
        // start with a black screen.
        if (showBlack)
        {
            images.alpha = 1.0f;
        }
        else
        {
            images.alpha = 0.0f;
        }

        if (autoToggleSelfOnce) {
            Invoke("toggleShowBlack", autoToggleDelay);
        }

    }

    void Update()
    {
        if (!transitioning && showBlack != showBlackPrevState)
        {
            if (showBlack && images.alpha <= 0.95f)
            {
                StartCoroutine(FadeCanvas(images, 0f, 1f, imageFadeDuration));

            }
            else
            {
                StartCoroutine(FadeCanvas(images, 1f, 0f, imageFadeDuration));
            }
        }

    }

    IEnumerator FadeCanvas(CanvasGroup canvas, float startAlpha, float endAlpha, float duration)
    {
        transitioning = true;

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
        transitioning = false;
        showBlackPrevState = showBlack;
    }

    public void toggleShowBlack() {
        showBlack = !showBlack;
    }
}
