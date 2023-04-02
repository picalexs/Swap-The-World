using UnityEditor;
using UnityEngine;

public class ButtonDoor : MonoBehaviour
{
    [SerializeField] private GameObject[] doorObjects;
    [HideInInspector] private EntranceDoor[] doors;
    private float objectsOnButton;

    [SerializeField] private Sprite openSprite;
    [SerializeField] private Sprite closedSprite;
    [SerializeField] private AudioSource buttonOn;
    [SerializeField] private AudioSource buttonOff;
    private SpriteRenderer spriteRenderer;
    private void Start()
    {
        doors = new EntranceDoor[doorObjects.Length];
        for (int i = 0; i < doorObjects.Length; i++)
        {
            doors[i] = doorObjects[i].GetComponent<EntranceDoor>();
        }
        spriteRenderer = GetComponent<SpriteRenderer>();
    }
    private void OnCollisionEnter2D(Collision2D other)
    {
        if(other.gameObject.tag == "Player" || other.gameObject.tag=="Swapable")
        {
            objectsOnButton++;
            if (objectsOnButton >= 1)
            {
                foreach (EntranceDoor door in doors)
                {
                    if (!door.isOpen)
                    {
                        door.OpenDoor();
                    }
                }
                spriteRenderer.sprite = openSprite;
                buttonOn.Play();
            } 
        }
    }

    private void OnCollisionExit2D(Collision2D other)
    {
        if (other.gameObject.tag == "Player" || other.gameObject.tag == "Swapable")
        {
            objectsOnButton--;
            if (objectsOnButton == 0)
            {
                foreach (EntranceDoor door in doors)
                {
                    if (door.isOpen)
                    {
                        door.CloseDoor();
                    }
                }
            }
        }
        spriteRenderer.sprite = closedSprite;
        buttonOff.Play();
    }
}
