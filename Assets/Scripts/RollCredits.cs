using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RollCredits : MonoBehaviour
{
    public Animator animator;
    public AudioSource soundPlayer;
    [SerializeField] private GameObject gameObjectToEnable;
    void Start()
    {
         soundPlayer.Play();
    }
    void Update()
    {
       
        StartCoroutine(PlayRollCredits());
        StartCoroutine(PlayRollCreditsButton());
    }
    IEnumerator PlayRollCredits()
    {
        yield return new WaitForSeconds(1f);
        animator.SetTrigger("roll");
        
    }
    IEnumerator PlayRollCreditsButton()
    {
        yield return new WaitForSeconds(15f);
        if (gameObjectToEnable.activeInHierarchy == false)
        {
            gameObjectToEnable.SetActive(true);
        }

    }
}
