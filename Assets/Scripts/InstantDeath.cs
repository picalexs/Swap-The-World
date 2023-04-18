using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InstantDeath : MonoBehaviour
{
    public GameObject abilityManager;
    private SwapAbility swapAbility;
    private PlayerScript playerScript;
    private void Start()
    {
        playerScript = FindObjectOfType<PlayerScript>();
        swapAbility = abilityManager.GetComponent<SwapAbility>();
    }
    private void OnCollisionEnter2D(Collision2D other)
    {
        if (other.collider.CompareTag("Player"))
        {
            if (playerScript._isSwapped)
            {
                swapAbility.ResetSwapPlayerObject();
            }
            playerScript.Die();
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            if (playerScript._isSwapped)
            {
                swapAbility.ResetSwapPlayerObject();
            }
            playerScript.Die();
        }
    }
}
