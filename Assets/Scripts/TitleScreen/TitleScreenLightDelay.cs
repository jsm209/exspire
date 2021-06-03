using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Experimental.Rendering.Universal;

// Once the delay is reached, fade in the light intensity
public class TitleScreenLightDelay : MonoBehaviour
{

    public float delay;
    public float targetIntensity;
    public float fadeSpeed;

    private Light2D myLight;
    private bool fadeIn;

    // Start is called before the first frame update
    void Start()
    {
        myLight = transform.GetComponent<Light2D>();
        fadeIn = false;

        myLight.intensity = 0f;
        Invoke("fadeInLight", delay);
    }

    private void fadeInLight() {
        fadeIn = true;
    }

    // Update is called once per frame
    void Update()
    {
        if (myLight.intensity < targetIntensity && fadeIn) {
            myLight.intensity = myLight.intensity + (Time.deltaTime * fadeSpeed);
        }
    }
}
