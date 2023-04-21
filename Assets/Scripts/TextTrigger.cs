using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
public class TextTrigger : MonoBehaviour
{
    [SerializeField] private GameObject gameObject;
    void Start()
    {
        gameObject.SetActive(false);
    }
   private void OnTriggerEnter2D(Collider2D collision) {
    if(collision.gameObject.CompareTag("Player"))
        {
            if(gameObject.activeInHierarchy==false) 
                {
                    gameObject.SetActive(true);
                }
            
        }
   }
   private void OnTriggerExit2D(Collider2D collision) {
    if(collision.gameObject.CompareTag("Player"))
        {
            if(gameObject.activeInHierarchy==true) 
                gameObject.SetActive(false);      
        }
   }
}
