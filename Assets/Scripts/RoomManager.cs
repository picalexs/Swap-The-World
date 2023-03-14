using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoomManager : MonoBehaviour
{
    public GameObject roomCamera;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.tag == "Player" && !other.isTrigger)
        {
            roomCamera.SetActive(true);
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.tag == "Player" && !other.isTrigger)
        {
            roomCamera.SetActive(false);
        }
    }
}
