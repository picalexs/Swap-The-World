using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpriteChanger : MonoBehaviour
{
    public InteractableDoor interactableDoor;
    [SerializeField] private Sprite SpriteOpen,SpriteClose;
     private void Start() {
        if(interactableDoor.doorType==1)
        {
            GetComponent<SpriteRenderer>().sprite=SpriteOpen;
        }
        else
        {
            if(PlayerDataSave.doorKey>=interactableDoor.doorKeyLevel)
        {
            GetComponent<SpriteRenderer>().sprite=SpriteOpen;
            }
        else
        {
            GetComponent<SpriteRenderer>().sprite=SpriteClose;
        }
        }
     }
    // Update is called once per frame
}
