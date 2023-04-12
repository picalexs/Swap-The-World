using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoomManager : MonoBehaviour
{
    public GameObject roomCamera;
    public GameObject roomElements;
    public float disableTime = 5f; // adjust this value to set the time delay
    private bool playerInRoom = false;
    private float disableTimer = 0f;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player") && !other.isTrigger)
        {
            playerInRoom = true;
            roomCamera.SetActive(true);
            roomElements.SetActive(true);
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player") && !other.isTrigger)
        {
            playerInRoom = false;
            disableTimer = disableTime;
            roomCamera.SetActive(false);
        }
    }

    private void Update()
    {
        if (!playerInRoom && disableTimer > 0)
        {
            disableTimer -= Time.deltaTime;
            if (disableTimer <= 0)
            {
                roomElements.SetActive(false);
            }
        }
    }
}
