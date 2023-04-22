using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class WriteTextLike : MonoBehaviour
{
    [SerializeField] private string textWrite;
    [SerializeField] private float delayBetweenLetters = 0.05f;
    [SerializeField] private float volume = 1.0f;
    [SerializeField] private float pitchMultiplier = 1.0f;
    [SerializeField] private float minPitch = 1.0f;
    [SerializeField] private float maxPitch = 2.0f;
    [SerializeField] private float commaPause = 0.5f;
    [SerializeField] private float periodPause = 1.0f;
    [SerializeField] private AudioClip[] audioClips;
    [SerializeField] private TMP_Text messageText;
    [SerializeField] private GameObject activateOnFinish;
    private AudioSource audioSource;
    private bool isWriting = true;

    private void Awake()
    {
        if (!messageText)
        {
            messageText = GameObject.Find("TextZone").GetComponent<TMP_Text>();
        }
        audioSource = GetComponent<AudioSource>();
    }

    private void Start()
    {
        if (!messageText)
        {
            Debug.LogWarning("No text object found.");
            return;
        }
        StartCoroutine(WriteTextWithSoundEffect());
    }

    private void Update()
    {
        if (!messageText)
        {
            return;
        }
        if (isWriting && Input.GetKeyDown(KeyCode.Space))
        {
            StopAllCoroutines();
            messageText.text = textWrite;
            isWriting = false;
            if (activateOnFinish)
            {
                activateOnFinish.SetActive(true);
            }
        }
    }

    IEnumerator WriteTextWithSoundEffect()
    {
        if (!messageText)
        {
            yield break;
        }
        messageText.text = "";
        int letterIndex = 0;

        while (letterIndex < textWrite.Length)
        {
            char letter = textWrite[letterIndex];
            int pitch = char.ToUpper(letter) - 'A' + 1; // Map letter to pitch (A=1, B=2, C=3, etc.)
            AudioClip clip = audioClips[Random.Range(0, audioClips.Length)];

            // Make sure pitch is greater than zero before setting it on the AudioSource
            pitch = Mathf.Max(pitch, 1);

            // Clamp the pitch value between minPitch and maxPitch
            float clampedPitch = Mathf.Clamp(pitch * pitchMultiplier, minPitch, maxPitch);

            audioSource.pitch = clampedPitch;
            audioSource.volume = volume;

            // Add pauses after commas and periods
            float pauseLength = delayBetweenLetters;
            if (letter == ',')
            {
                pauseLength = commaPause;
            }
            else if (letter == '.')
            {
                pauseLength = periodPause;
            }

            audioSource.PlayOneShot(clip);
            messageText.text += letter;
            letterIndex++;
            yield return new WaitForSeconds(pauseLength);
        }

        // Once all the text has been written, add a delay before playing the sound effect one last time
        yield return new WaitForSeconds(0.5f);

        // Play the sound effect one last time with a pitch of 1 (to indicate the end of the text)
        audioSource.pitch = 1 * pitchMultiplier;
        AudioClip endClip = audioClips[Random.Range(0, audioClips.Length)];
        audioSource.PlayOneShot(endClip, volume);

        isWriting = false;
        if (activateOnFinish)
        {
            activateOnFinish.SetActive(true);
        }
    }
}
