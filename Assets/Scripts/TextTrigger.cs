using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
public class TextTrigger : MonoBehaviour
{
    [SerializeField] private GameObject textTrigger;
    void Start()
    {
        textTrigger.SetActive(false);
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            if (textTrigger.activeInHierarchy == false)
            {
                textTrigger.SetActive(true);
            }

        }
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            if (textTrigger.activeInHierarchy == true)
                textTrigger.SetActive(false);
        }
    }
}
