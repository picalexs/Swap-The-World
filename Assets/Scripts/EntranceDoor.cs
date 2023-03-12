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
            transform.position = Vector3.Lerp(transform.position, closedPosition, doorSpeed * Time.deltaTime);
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
}
