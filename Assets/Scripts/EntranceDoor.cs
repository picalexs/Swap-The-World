using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class EntranceDoor : MonoBehaviour
{
    [SerializeField] private float doorHeight = 2.0f;
    [SerializeField] private float doorSpeed = 2.0f;

    private Vector3 closedPosition;
    private Vector3 openPosition;
    private bool isOpen = false;
    private bool isBlocked = false;

    [SerializeField] private Sprite closedSprite;
    [SerializeField] private Sprite openSprite;
    private SpriteRenderer spriteRenderer;
    private void Start()
    {
        closedPosition = transform.position;
        openPosition = closedPosition + Vector3.up * doorHeight;
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    private void Update()
    {
        if (isOpen)
        {
            transform.position = Vector3.Lerp(transform.position, openPosition, doorSpeed * Time.deltaTime);
        }
        else
        {
            if (!isBlocked)
            {
                transform.position = Vector3.Lerp(transform.position, closedPosition, doorSpeed * Time.deltaTime);
            }
        }
    }

    public void OpenDoor()
    {
        isOpen = true;
        spriteRenderer.sprite = openSprite;
    }

    public void CloseDoor()
    {
        isOpen = false;
        spriteRenderer.sprite = closedSprite;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if(collision.gameObject.tag == "Player" || collision.gameObject.tag == "Swapable" || collision.gameObject.tag == "Rewindable")
        {
            isBlocked = true;
        }
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        isBlocked = false;
    }
}
