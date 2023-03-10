using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Interactable : MonoBehaviour
{
    public bool isInRange;
    public KeyCode interactKey;
    public UnityEvent interactAction;
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(isInRange)
        {
            if(Input.GetKeyDown(interactKey))
            {
                interactAction.Invoke();
            }
        }
    }
    private void OnTriggerEnter2D(Collider2D collision) {
        if(collision.gameObject.CompareTag("Player"))
        {
            isInRange=true;
            Debug.Log("player is in range");
        }
    }
    private void OnTriggerExit2D(Collider2D collision) {
        if(collision.gameObject.CompareTag("Player"))
        {
            isInRange=false;
            Debug.Log("player is not in range");
        }
    }
}
