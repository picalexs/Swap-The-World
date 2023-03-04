using UnityEngine;

public class SetSpawnPoint : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
        var player = collision.GetComponent<PlayerScript>();
        if (player != null)
        {
            player.SetRespawnPoint(transform.position);
        }
    }
}
