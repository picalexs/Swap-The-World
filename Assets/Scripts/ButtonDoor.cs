using UnityEngine;

public class ButtonDoor : MonoBehaviour
{
    [SerializeField] private GameObject doorObject;
    [HideInInspector] private EntranceDoor door;

    [SerializeField] private Sprite openSprite;
    [SerializeField] private Sprite closedSprite;
    private SpriteRenderer spriteRenderer;
    private void Start()
    {
        door = doorObject.GetComponent<EntranceDoor>();
        spriteRenderer = GetComponent<SpriteRenderer>();
    }
    private void OnCollisionEnter2D(Collision2D other)
    {
        if(other.gameObject.tag == "Player" || other.gameObject.tag=="Swapable")
        {
            door.OpenDoor();
            spriteRenderer.sprite = openSprite;
        }
    }

    private void OnCollisionExit2D(Collision2D other)
    {
        if (other.gameObject.tag == "Player" || other.gameObject.tag == "Swapable")
        {
            door.CloseDoor();
            spriteRenderer.sprite = closedSprite;
        }
    }
}
