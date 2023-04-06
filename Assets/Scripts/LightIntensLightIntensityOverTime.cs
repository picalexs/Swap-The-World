using UnityEngine;
using UnityEngine.Experimental.Rendering.Universal;
using UnityEngine.Rendering.Universal;

public class LightIntensityOverTime : MonoBehaviour
{
    public float spotlightIntensity = 1f;
    public float duration = 10f;
    public float fadeDuration = 2f;
    public float startTime;
    private Light2D spotlight;
    private float initialIntensity;

    private void Start()
    {
        spotlight = GetComponent<Light2D>(); 
        startTime = Time.time;
        initialIntensity = spotlight.intensity;
    }

    private void Update()
    {
        float elapsed = Time.time - startTime;
        float progress = elapsed / duration;
        if (progress > 1f - fadeDuration / duration)
        {
            float fadeProgress = (progress - (1f - fadeDuration / duration)) / (fadeDuration / duration);
            spotlight.intensity = Mathf.Lerp(spotlightIntensity, 0f, fadeProgress);
        }
        else
        {
            spotlight.intensity = initialIntensity;
        }
    }
}
