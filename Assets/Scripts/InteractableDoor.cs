using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

public class InteractableDoor : MonoBehaviour
{
    [SerializeField] private bool inRange = false;
    public KeyCode interactKey;
    public UnityEvent interactAction;
    [SerializeField] public int doorType = 0; // 0: normal door; 1: exit door
    [SerializeField] public int doorKeyLevel = 0;
     public Animator animationFinale;
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
    public void OpenGameScene(int i)
    {
       StartCoroutine(OpenGamescenefor(i));
    }
    IEnumerator OpenGamescenefor(int i ) {
        PlayerDataSave.SaveData();
        PlayerDataSave.LoadData();
        animationFinale.SetTrigger("end");
        yield return new WaitForSeconds(0.6f);
        SceneManager.LoadScene(i);
        
    }
}
