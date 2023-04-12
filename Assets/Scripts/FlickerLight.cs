using UnityEngine;
using System.Collections;
using UnityEngine.Rendering.Universal;

public class FlickerLight : MonoBehaviour
{
    public Light2D torchLight;
    public float minIntensity = 0.3f;
    public float maxIntensity = 0.8f;
    public float flickerSpeed = 0.1f;
    IEnumerator Flicker()
    {
        while (true)
        {
            float flickerIntensity = Random.Range(minIntensity, maxIntensity);
            torchLight.intensity = flickerIntensity;
            yield return new WaitForSeconds(flickerSpeed);
        }
    }

    private void OnEnable()
    {
        StartCoroutine(Flicker());
    }
}
