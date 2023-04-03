using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class InteractableDoor : MonoBehaviour
{
    [SerializeField] private bool inRange = false;
    public KeyCode interactKey;
    public UnityEvent interactAction;
    [SerializeField] private int doorType = 0; // 0: normal door; 1: exit door
    [SerializeField] private int doorKeyLevel = 0;

    public void Update()
    {
        if (!inRange) {
            return;
        }
        if (Input.GetKeyDown(interactKey))
        {
            if (doorType == 0)
            {
                if (PlayerDataSave.doorKey >= doorKeyLevel)
                {
                    Debug.Log("entered");
                    interactAction.Invoke();
                }
                else
                {
                    Debug.Log("cant enter!");
                }
            }
            else if (doorType == 1)
            {
                PlayerDataSave.ChangeKeyTo(PlayerDataSave.doorKey + 1);
                Debug.Log("upgraded key to: " + PlayerDataSave.doorKey);
                interactAction.Invoke();
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D collision) {
        if(collision.gameObject.CompareTag("Player"))
        {
            inRange = true;
            Debug.Log("in range");
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            inRange = false;
            Debug.Log("not in range");
        }
    }
}
