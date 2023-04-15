using Unity.VisualScripting;
using UnityEngine;
using System.Collections;

public class Spring : MonoBehaviour
{
    [SerializeField] private PlayerScript playerScript;
    [SerializeField] private float springForcePlayer = 20f;
    [SerializeField] private float springForceBox = 50f;
    [SerializeField] private AudioSource springSound;
    private Vector2 jumpDir;
    [SerializeField] private float freezeCooldown = 1f;
    [SerializeField] private float playerRetractSpringCooldown = 1f;
    [SerializeField] private float boxRetractSpringCooldown = 0.5f;
    private bool canMove = true;
    private bool canJump = true;
    [SerializeField] private Vector2 springAngleOffset;
    private Animator animator;

    private void Start()
    {
        springSound = GetComponent<AudioSource>();
        animator = GetComponent<Animator>();
    }
    private void OnTriggerStay2D(Collider2D collision)
    {
        if (!canMove || !canJump)
        {
            return;
        }

        Rigidbody2D[] rbs = collision.GetComponents<Rigidbody2D>();
        if (rbs == null)
        {
            return;
        }

        foreach (Rigidbody2D rb in rbs)
        {
            jumpDir = transform.up;
            if (collision.CompareTag("Player"))
            {
                StartCoroutine(RetractSpring(playerRetractSpringCooldown));
                if (jumpDir.y > 0f)
                {
                    Debug.Log("can not jump");
                    StartCoroutine(DisableJump());
                    jumpDir.x += springAngleOffset.x;
                }
                else if (Mathf.Abs(jumpDir.x) > 0f)
                {
                    Debug.Log("can not move");
                    StartCoroutine(DisableMovement());
                    jumpDir.y = Mathf.Abs(jumpDir.y) + springAngleOffset.y;
                }
                springSound.Stop();
                springSound.Play();
                rb.velocity = new Vector2(0f, 0f);
                rb.AddForce(jumpDir * springForcePlayer, ForceMode2D.Impulse);
            }
            else if (collision.CompareTag("Swappable"))
            {
                StartCoroutine(RetractSpring(boxRetractSpringCooldown));
                springSound.Stop();
                springSound.Play();
                rb.velocity = new Vector2(0f, 0f);
                if (playerScript._isSwapped == true)
                {
                    rb.AddForce(jumpDir * springForcePlayer, ForceMode2D.Impulse);
                }
                else
                {
                    rb.AddForce(jumpDir * springForceBox, ForceMode2D.Impulse);
                }
            }
        }
    }
    private IEnumerator RetractSpring(float retractSpringDuration)
    {
        animator.SetBool("Active", true);
        yield return new WaitForSeconds(retractSpringDuration);
        animator.SetBool("Active", false);
    }

    private IEnumerator DisableMovement()
    {
        canMove = false;
        playerScript._canMove = false;
        yield return new WaitForSeconds(freezeCooldown);
        canMove = true;
        playerScript._canMove = true;
        Debug.Log("can move");
    }

    private IEnumerator DisableJump()
    {
        canJump = false;
        playerScript._canJump = false;
        yield return new WaitForSeconds(freezeCooldown);
        canJump = true;
        playerScript._canJump = true;
        Debug.Log("can jump");
    }
}
