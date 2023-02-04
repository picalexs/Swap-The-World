using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.InputSystem;

public class PlayerScript : MonoBehaviour
{
    public Rigidbody2D rb;
    public Transform groundCheck;
    public LayerMask groundLayer;

    private float horizontal;
    [SerializeField]
    private float speed = 8f;
    [SerializeField]
    private float jumpingPower = 16f;
    [SerializeField]
    private bool isFacingRight = true;

    [SerializeField]
    private bool jumpQueue;
    [SerializeField]
    private float firstJumpPress,lastGrounded;
    [SerializeField]
    private float preGroundJumpCooldown,preAirJumpCooldown;

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
        if (IsGrounded())
        {
            if (Time.time - firstJumpPress <= preGroundJumpCooldown && jumpQueue)
            {
                rb.velocity = new Vector2(rb.velocity.x, jumpingPower);
            }
            jumpQueue = false;
        }
        rb.velocity = new Vector2(horizontal * speed, rb.velocity.y);
    }

    public void Jump(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            if (IsGrounded() || Time.time - lastGrounded <= preAirJumpCooldown)
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

    private bool IsGrounded()
    {
        bool grounded = Physics2D.OverlapCircle(groundCheck.position, 0.2f, groundLayer);
        if (grounded)
        {
            lastGrounded = Time.time;
        }
        return grounded;
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
