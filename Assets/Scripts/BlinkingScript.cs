using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class BlinkingScript : MonoBehaviour
{
    public bool test = false;
    public float blinkDuration = 0.1f;
    public float baseBlinkFrequency = 0.1f;
    public float abilityDuration = 3.0f;
    public float blinkStartThreshold = 1.0f;
    public float blinkEndThreshold = 0.5f;
    public float maxBlinkFrequency = 0.05f;

    private Material playerMaterial;
    public Color playerBlinkColor;
    private float timeLeft;
    private bool isBlinking;

    void Start()
    {
        UpdateMaterial();
    }

    public void UpdateMaterial()
    {
        playerMaterial = GetComponent<Renderer>().material;
        Debug.Log("playerMat " + playerMaterial.name);
        timeLeft = abilityDuration;
        isBlinking = false;
    }

    public void StartBlinking()
    {
        UpdateMaterial();
        if (!isBlinking)
        {
            isBlinking = true;
            StartCoroutine(BlinkCoroutine(baseBlinkFrequency));
        }
    }

    void Update()
    {
        if (test)
        {
            StartBlinking();
            test = false;
        }
        
        if (isBlinking)
        {
            timeLeft -= Time.deltaTime;
            if (timeLeft <= blinkEndThreshold)
            {
                float blinkFrequency = Mathf.Lerp(baseBlinkFrequency, maxBlinkFrequency, 1 - timeLeft / blinkEndThreshold);
                StartCoroutine(BlinkCoroutine(blinkFrequency));
            }
            if (timeLeft <= 0)
            {
                StopAbility();
            }
        }
    }

    IEnumerator BlinkCoroutine(float blinkFrequency)
    {
        while (isBlinking)
        {
            Debug.Log("blink");
            playerMaterial.SetColor("_HitEffectColor", playerBlinkColor);
            playerMaterial.SetFloat("_HitEffectBlend", 1f);
            playerMaterial.SetFloat("_ChromAberrAmount", 0.4f);
            yield return new WaitForSeconds(blinkDuration);
            playerMaterial.SetFloat("_HitEffectBlend", 0f);
            playerMaterial.SetFloat("_ChromAberrAmount", 0f);
            yield return new WaitForSeconds(blinkFrequency);
        }
    }

    void StopAbility()
    {
        Debug.Log("stop blinking");
        StopCoroutine(BlinkCoroutine(baseBlinkFrequency));
        isBlinking = false;
        playerMaterial.SetFloat("_HitEffectBlend", 0f);
        playerMaterial.SetFloat("_ChromAberrAmount", 0f);
        timeLeft = abilityDuration;
    }
}
