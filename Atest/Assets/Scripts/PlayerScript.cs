using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.InputSystem;

public class PlayerScript : MonoBehaviour
{
    private Rigidbody2D rb;
    private Collission collission;

    [SerializeField]
    private float horizontal;
    [SerializeField]
    private bool isFacingRight = true;

    [SerializeField]
    private float speed = 8f, climbingSpeed = 8f, climbingModifier=0.35f, stickCooldown =0.2f, s_cooldown;
    [SerializeField]
    private float jumpingPower = 16f,gravityScale=2.5f;


    [SerializeField]
    private bool jumpQueue;
    [SerializeField]
    private float firstJumpPress;
    [SerializeField]
    private float preGroundJumpCooldown=0.15f,coyoteCooldown=0.15f,wallCooldown=0.15f;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        collission = GetComponent<Collission>();
    }

    void Update()
    {
        if (!isFacingRight && horizontal > 0f)
        {
            Flip();
        }
        else if (isFacingRight && horizontal < 0f)
        {
            Flip();
        }
    }

    private void FixedUpdate()
    {
        if(s_cooldown > 0)
            s_cooldown -= Time.deltaTime;

        if (collission.IsGrounded())
        {
            if (Time.time - firstJumpPress <= preGroundJumpCooldown && jumpQueue)
            {
                rb.velocity = new Vector2(rb.velocity.x, jumpingPower);
            }
            jumpQueue = false;
        }
        if (collission.onWall() && s_cooldown <=0)
        {
            if (rb.gravityScale != 0)
            { 
                rb.gravityScale = 0;
                rb.velocity = new Vector2(rb.velocity.x, 0);
            }
           /* 
            float speedModifier = rb.velocity.y > 0 ? climbingModifier : 1f;
            rb.velocity = new Vector2(horizontal * speed, rb.velocity.y * climbingModifier * climbingSpeed);*/
        }
        else
        {
            rb.gravityScale = gravityScale;
        }
        rb.velocity = new Vector2(horizontal * speed, rb.velocity.y);
    }

    public void Jump(InputAction.CallbackContext context)
    {
        float lastGrounded = collission.lastGrounded;
        float lastWall = collission.lastWall;

        if (context.performed)
        {
            if(collission.onWall() || Time.time - lastWall <= wallCooldown)
            {
                s_cooldown = stickCooldown;
                rb.gravityScale = gravityScale;
                rb.velocity = new Vector2(rb.velocity.x, jumpingPower);
            }
            else if (collission.IsGrounded() || Time.time - lastGrounded <= coyoteCooldown)
            {
                rb.velocity = new Vector2(rb.velocity.x, jumpingPower);
            }
            else
            {
                if (!jumpQueue)
                {
                    firstJumpPress = Time.time;
                    jumpQueue = true; 
                }
            }
        }

        if (context.canceled && rb.velocity.y > 0f)
        {
            rb.velocity = new Vector2(rb.velocity.x, rb.velocity.y * 0.5f);
        }
    }
    private void Flip()
    {
        isFacingRight = !isFacingRight;
        Vector3 localScale = transform.localScale;
        localScale.x *= -1f;
        transform.localScale = localScale;
    }

    public void Move(InputAction.CallbackContext context)
    {
        horizontal = context.ReadValue<Vector2>().x;
    }
}
