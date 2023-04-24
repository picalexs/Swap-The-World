using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class EntranceDoor : MonoBehaviour
{
    [SerializeField] private float doorSpeed = 2.0f;
    [SerializeField] private Vector3 pointA;
    [SerializeField] private Vector3 pointB;

    public bool isOpen = false;
    private bool isBlocked = false;

    [SerializeField] private Sprite closedSprite;
    [SerializeField] private Sprite openSprite;
    private SpriteRenderer spriteRenderer;

    private void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    private void Update()
    {
        Vector3 targetPosition = isOpen ? pointB : pointA;
        transform.position = Vector3.MoveTowards(transform.position, targetPosition, doorSpeed * Time.deltaTime);
    }

    public void OpenDoor()
    {
        isOpen = true;
        spriteRenderer.sprite = openSprite;
    }

    public void CloseDoor()
    {
        if (!isBlocked)
        {
            isOpen = false;
            spriteRenderer.sprite = closedSprite;
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player") || collision.gameObject.CompareTag("Swappable") || collision.gameObject.CompareTag("Rewindable"))
        {
            ContactPoint2D[] contacts = new ContactPoint2D[collision.contactCount];
            collision.GetContacts(contacts);
            //chek to see if the player is touching the door from below
            foreach (ContactPoint2D contact in contacts)
            {
                if (contact.normal.y > 0.5f)
                {
                    isBlocked = true;
                    break;
                }
            }
        }
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        isBlocked = false;
    }
}
