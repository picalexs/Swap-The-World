using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using UnityEngine;

public class RewindTimeAbility : MonoBehaviour
{
    public float rewindTime = 2.0f;
    public float slowDownFactor = 0.1f;
    public float maxSlowdownDuration = 2f;
    public float slowMotionTimeLeft = 0f;
    private float originalFixedDeltaTime;
    public KeyCode rewindKey = KeyCode.R;
    public List<string> tagsToRewind;

    private bool isRewinding = false;
    [HideInInspector] public bool isSlowingDown = false;
    private List<Vector2> playerPositions = new List<Vector2>();
    private Dictionary<GameObject, List<Vector2>> objectPositions = new Dictionary<GameObject, List<Vector2>>();
    [SerializeField, Description("Player")] private GameObject playerObject;
    private Rigidbody2D rb2d;

    void Start()
    {
        originalFixedDeltaTime = Time.fixedDeltaTime;
        rb2d = playerObject.GetComponent<Rigidbody2D>();
    }

    void Update()
    {
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
        if (isRewinding)
        {
            if (playerPositions.Count > 0)
            {
                Vector2 lastPlayerPosition = playerPositions[playerPositions.Count - 1];
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
                    Vector2 lastPosition = positions[positions.Count - 1];
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
                    Rigidbody2D objRb2d = obj.GetComponent<Rigidbody2D>();
                    if (objRb2d != null)
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
        Invoke("StopRewind", rewindTime);
    }
    void StopRewind()
    {
        isRewinding = false;
        rb2d.isKinematic = false;
        objectPositions.Clear();
    }

    public void SlowTimeDown()
    {
        isSlowingDown = true;
        slowMotionTimeLeft = maxSlowdownDuration;
        StartCoroutine(SlowTime());
    }

    public void CancelSlowDown()
    {
        slowMotionTimeLeft = 0;
    }
    IEnumerator SlowTime()
    {
        isSlowingDown = true;
        Time.timeScale = slowDownFactor;
        Time.fixedDeltaTime = originalFixedDeltaTime * slowDownFactor;

        while (slowMotionTimeLeft > 0f)
        {
            yield return new WaitForSecondsRealtime(0.1f);
            slowMotionTimeLeft -= 0.1f;
        }

        Time.timeScale = 1f;
        Time.fixedDeltaTime = originalFixedDeltaTime;
        slowMotionTimeLeft = 0f;
        isSlowingDown = false;
    }
}
