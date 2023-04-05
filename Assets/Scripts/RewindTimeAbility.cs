using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using UnityEngine;
using UnityEngine.Audio;

public class RewindTimeAbility : MonoBehaviour
{
    [SerializeField, Description("Player")] private GameObject playerObject;
    private Rigidbody2D rb2d;

    [SerializeField] private AudioMixerGroup audioMixerGroup;

    [SerializeField] private float slowDownFactor = 0.25f;
    [SerializeField] private float transitionDuration = 1f;
    public float maxSlowdownDuration = 3f;
    public float slowMotionTimeLeft = 0f;
    private float originalPitch;

    [SerializeField] private bool doRewind = false;
    private bool isRewinding = false;
    [SerializeField] private float rewindTime = 2.0f;
    [SerializeField] private KeyCode rewindKey = KeyCode.R;
    [SerializeField] private List<string> tagsToRewind;

    private readonly List<Vector2> playerPositions = new();
    private readonly Dictionary<GameObject, List<Vector2>> objectPositions = new();
    void Start()
    {
        rb2d = playerObject.GetComponent<Rigidbody2D>();
        audioMixerGroup .audioMixer.GetFloat("Pitch", out originalPitch);
    }

    void Update()
    {
        if (!doRewind)
        {
            return;
        }
        if (Input.GetKeyDown(rewindKey))
        {
            StartRewind();
        }
        if (Input.GetKeyUp(rewindKey))
        {
            StopRewind();
        }
    }

    void FixedUpdate()
    {
        if (!doRewind)
        {
            return;
        }
        if (isRewinding)
        {
            if (playerPositions.Count > 0)
            {
                Vector2 lastPlayerPosition = playerPositions[^1];
                rb2d.MovePosition(lastPlayerPosition);
                playerPositions.RemoveAt(playerPositions.Count - 1);
            }
            else
            {
                StopRewind();
            }

            // Move other objects back in time as well
            foreach (KeyValuePair<GameObject, List<Vector2>> pair in objectPositions)
            {
                GameObject obj = pair.Key;
                List<Vector2> positions = pair.Value;

                if (positions.Count > 0)
                {
                    Vector2 lastPosition = positions[^1];
                    Rigidbody2D objRb2d = obj.GetComponent<Rigidbody2D>();
                    objRb2d.MovePosition(lastPosition);
                    positions.RemoveAt(positions.Count - 1);
                }
            }
        }
        else
        {
            playerPositions.Add(playerObject.transform.position);

            foreach (string tag in tagsToRewind)
            {
                GameObject[] objects = GameObject.FindGameObjectsWithTag(tag);
                foreach (GameObject obj in objects)
                {
                    if (obj.TryGetComponent<Rigidbody2D>(out var objRb2d))
                    {
                        if (!objectPositions.ContainsKey(obj))
                        {
                            objectPositions[obj] = new List<Vector2>();
                        }
                        objectPositions[obj].Add(objRb2d.position);
                    }
                }
            }
        }
    }
    void StartRewind()
    {
        isRewinding = true;
        rb2d.isKinematic = true;
        Invoke(nameof(StopRewind), rewindTime);
    }
    void StopRewind()
    {
        isRewinding = false;
        rb2d.isKinematic = false;
        objectPositions.Clear();
    }

    public void SlowTimeDown()
    {
        slowMotionTimeLeft = maxSlowdownDuration;
        StartCoroutine(SlowTime());
    }

    public void CancelSlowDown()
    {
        slowMotionTimeLeft = 0;
    }
    IEnumerator SlowTime()
    {
        float initialFixedDeltaTime = Time.fixedDeltaTime;

        float t = 0f;
        while (t < 1f)
        {
            t += Time.unscaledDeltaTime / transitionDuration;
            Time.timeScale = Mathf.Lerp(Time.timeScale, slowDownFactor, t);
            Time.fixedDeltaTime = Mathf.Lerp(initialFixedDeltaTime, initialFixedDeltaTime * slowDownFactor, t);
            audioMixerGroup.audioMixer.SetFloat("Pitch", Mathf.Lerp(originalPitch, slowDownFactor, t));
            yield return null;
        }

        while (slowMotionTimeLeft > 0f)
        {
            yield return new WaitForSecondsRealtime(0.1f);
            slowMotionTimeLeft -= 0.1f;
        }

        t = 0;
        while (t < 1f)
        {
            t += Time.unscaledDeltaTime / transitionDuration;
            Time.timeScale = Mathf.Lerp(slowDownFactor, 1f, t);
            Time.fixedDeltaTime = Mathf.Lerp(initialFixedDeltaTime * slowDownFactor, initialFixedDeltaTime, t);
            audioMixerGroup.audioMixer.SetFloat("Pitch", Mathf.Lerp(slowDownFactor, originalPitch, t));
            yield return null;
        }

        slowMotionTimeLeft = 0f;
    }
}
