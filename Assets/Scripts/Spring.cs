using Unity.VisualScripting;
using UnityEngine;
using System.Collections;

public class Spring : MonoBehaviour
{
    [SerializeField] private PlayerScript playerMovement;
    [SerializeField] private float springForce = 10f;
    [SerializeField] private AudioSource springSound;
    private Vector2 jumpDir;
    [SerializeField] private float freezeCooldown = 1f;
    [SerializeField] private float retractSpringCooldown = 1f;
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
        if (collision.CompareTag("Player") && canMove && canJump)
        {
            Rigidbody2D rb = collision.GetComponent<Rigidbody2D>();
            if (rb != null)
            {
                StartCoroutine(RetractSpring());
                jumpDir = transform.up;
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
                springSound.Play();
                rb.velocity = new Vector2(0f, 0f);
                rb.AddForce(jumpDir * springForce, ForceMode2D.Impulse);
            }
        }
    }
    private IEnumerator RetractSpring()
    {
        animator.SetBool("Active", true);
        yield return new WaitForSeconds(retractSpringCooldown);
        animator.SetBool("Active", false);
    }

    private IEnumerator DisableMovement()
    {
        canMove = false;
        playerMovement._canMove = false;
        yield return new WaitForSeconds(freezeCooldown);
        canMove = true;
        playerMovement._canMove = true;
        Debug.Log("can move");
    }

    private IEnumerator DisableJump()
    {
        canJump = false;
        playerMovement._canJump = false;
        yield return new WaitForSeconds(freezeCooldown);
        canJump = true;
        playerMovement._canJump = true;
        Debug.Log("can jump");
    }
}
