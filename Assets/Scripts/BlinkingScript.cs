using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlinkingScript : MonoBehaviour
{
    public bool testBlinking = false;

    [SerializeField] private GameObject playerObject;
    [SerializeField] private AudioSource blinkSound;
    public float baseBlinkDuration = 0.1f;
    public float baseBlinkFrequency = 0.1f;
    public float abilityDuration = 3.0f;
    public float blinkEndThreshold = 0.5f;
    public float endBlinkFrequency = 0.5f;
    public float endBlinkDuration = 0.5f;

    private Material playerMaterial;
    [SerializeField] private Color playerBlinkColor = new(255, 76, 215);
    [SerializeField] private float playerGlowAmount = 5f;
    [SerializeField] private float maxBlendAmount = 0.8f;
    [SerializeField] private float chromAberrAmount = 5f;

    private float timeLeft;
    private bool isBlinking;
    private bool isSwaped = false;

    void Start()
    {
        UpdateMaterial();
    }

    public void UpdateMaterial()
    {
        Debug.Log("!updating materials");
        playerMaterial = playerObject.GetComponent<Renderer>().material;
        playerMaterial.SetColor("_HitEffectColor", playerBlinkColor);
        playerMaterial.SetFloat("_HitEffectGlow", playerGlowAmount);
        timeLeft = abilityDuration;
        isBlinking = false;
    }

    public void StartBlinking()
    {
        UpdateMaterial();
        if (!isBlinking)
        {
            isBlinking = true;
            StartCoroutine(BlinkCoroutine(baseBlinkFrequency, baseBlinkDuration, endBlinkFrequency, endBlinkDuration));
        }
    }

    void Update()
    {
        if (testBlinking)
        {
            StartBlinking();
            testBlinking = false;
        }

        if (isBlinking)
        {
            timeLeft -= Time.deltaTime;
            if (timeLeft <= 0)
            {
                StopAbility();
            }
        }
    }

    IEnumerator BlinkCoroutine(float startFrequency, float startDuration, float endFrequency, float endDuration)
    {
        float timer = 0f;
        while (isBlinking && timer < abilityDuration)
        {
            float progress = timer / abilityDuration;
            float currentFrequency = Mathf.Lerp(startFrequency, endFrequency, progress);
            float currentDuration = Mathf.Lerp(startDuration, endDuration, progress);
            float currentPitch = Mathf.Lerp(1.0f, 2.0f, progress);

            playerMaterial.SetFloat("_HitEffectBlend", maxBlendAmount);
            playerMaterial.SetFloat("_ChromAberrAmount", Random.Range(1, chromAberrAmount));
            blinkSound.pitch = currentPitch;
            blinkSound.Play();

            yield return new WaitForSeconds(currentDuration);

            playerMaterial.SetFloat("_HitEffectBlend", 0f);
            playerMaterial.SetFloat("_ChromAberrAmount", 0f);

            yield return new WaitForSeconds(currentFrequency - currentDuration);

            timer += currentFrequency;
        }

        StopAbility();
    }

    public void StopAbility()
    {
        Debug.Log("stop blinking");
        isBlinking = false;
        playerMaterial.SetFloat("_HitEffectBlend", 0f);
        playerMaterial.SetFloat("_ChromAberrAmount", 0f);
        timeLeft = abilityDuration;
    }

    public void ChangePlayerObjectTo(GameObject newObject)
    {
        Debug.Log("!changed playerobject to: " + newObject);
        playerObject = newObject;
        isSwaped = !isSwaped;
        if (isSwaped)
        {
            Debug.Log("!start blinking");
            StartBlinking();
        }
    }
}
