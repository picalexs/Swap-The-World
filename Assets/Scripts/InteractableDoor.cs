using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class InteractableDoor : MonoBehaviour
{
    public bool isInRange;
    public KeyCode interactKey;
    public UnityEvent interactAction;
    [SerializeField] private int levelNumber;
    [SerializeField] public int doors;
    [SerializeField] private GameObject player;
    [SerializeField] public bool LevelDoor;
    [SerializeField] public bool ExitDoor;
    [SerializeField] private bool previousLevelComplet;
    private void Start() {
        doors= player.GetComponent<PlayerDataSave>().doorsUnlocked;
        //player.GetComponent<PlayerDataSave>().LoadData();
        //player.GetComponent<PlayerDataSave>().ResetData();
    }
    void Update()
    {
        LevelDoor=player.GetComponent<PlayerDataSave>().isLevelDoor;
        ExitDoor=player.GetComponent<PlayerDataSave>().isExitDoor;
        previousLevelComplet= (doors>=levelNumber);
        if(isInRange)
        {
            if(LevelDoor)
            {
                if(previousLevelComplet)
                {
                    if(Input.GetKeyDown(interactKey))
                        {
                           
                                interactAction.Invoke();
                        } 
                }                
                else
                {
                    Debug.Log("nu poti intra");
                }
            }
            else if(ExitDoor)
            {
                if(Input.GetKeyDown(interactKey))
                    {
                    player.GetComponent<PlayerDataSave>().doorsUnlocked=Mathf.Max(player.GetComponent<PlayerDataSave>().doorsUnlocked,levelNumber+1);
                    Debug.Log(player.GetComponent<PlayerDataSave>().doorsUnlocked);
                    player.GetComponent<PlayerDataSave>().SaveData();
                    Debug.Log("am crescut");
                    interactAction.Invoke();
                    }         
            }
            }
            
        }
    private void OnTriggerEnter2D(Collider2D collision) {
        if(collision.gameObject.CompareTag("Player"))
        {
            isInRange=true;
            Debug.Log("on");
        }
    }
    private void OnTriggerExit2D(Collider2D collision) {
        if(collision.gameObject.CompareTag("Player"))
        {
            isInRange=false;
            Debug.Log("off");
        }
    }
}
