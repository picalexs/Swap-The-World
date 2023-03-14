using UnityEngine;

public class InstantDeath : MonoBehaviour
{
    private PlayerScript playerScript;
    private void Start()
    {
        playerScript = FindObjectOfType<PlayerScript>();
    }
    private void OnCollisionEnter2D(Collision2D other)
    {
        if (other.collider.tag == "Player")
        {
            playerScript.Die();
        }
    }
}
