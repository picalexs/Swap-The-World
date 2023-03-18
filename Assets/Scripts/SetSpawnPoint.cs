using UnityEngine;

public class SetSpawnPoint : MonoBehaviour
{
    private PlayerScript playerScript;
    private LayerMask groundLayer;
    private void Start()
    {
        playerScript = FindObjectOfType<PlayerScript>();
        groundLayer = LayerMask.GetMask("Ground", "Swappable");
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            Debug.Log("set spawnpoint");
            RaycastHit2D hit = Physics2D.Raycast(transform.position, Vector2.down,Mathf.Infinity,groundLayer);
            if (hit.collider != null && !hit.collider.isTrigger)
            {
                playerScript.SetRespawnPoint(new Vector2(transform.position.x, hit.point.y));
            }
        }
    }
}
