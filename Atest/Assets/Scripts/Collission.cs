using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Collission : MonoBehaviour
{
    private BoxCollider2D col;
    private Transform groundCheck;
    public LayerMask groundLayer;

    [SerializeField]
    private float groundCheckBox_height = 0.3f, //the height of the ground collider
                  groundCheckBox_length = 0.5f, //the amount to subtract from the length of the collider
                  wallCheckBox_height = 0.1f; //the amount to subtract from the bottom and from the top of the wall colliders
    public float lastGrounded,lastWall;
    
    private void Awake()
    {
        col = GetComponent<BoxCollider2D>();
        groundCheck = GetComponentInChildren<Transform>();
    }

    public bool IsGrounded()
    {
        Vector2 topLeftPoint = transform.position;
        topLeftPoint.x -= col.bounds.extents.x - groundCheckBox_length;
        topLeftPoint.y -= col.bounds.extents.y - groundCheckBox_height;

        Vector2 bottomRightPoint = transform.position;
        bottomRightPoint.x += col.bounds.extents.x-groundCheckBox_length;
        bottomRightPoint.y -= col.bounds.extents.y;

        bool grounded = Physics2D.OverlapArea(topLeftPoint, bottomRightPoint, groundLayer);
        if (grounded)
        {
            lastGrounded = Time.time;
        }
        return grounded;
    }

    public bool onWall()
    {
        Vector2 l_topLeftPoint = transform.position;
        l_topLeftPoint.x -= col.bounds.extents.x;
        l_topLeftPoint.y += col.bounds.extents.y-wallCheckBox_height;

        Vector2 l_bottomRightPoint = transform.position;
        l_bottomRightPoint.y -= col.bounds.extents.y - wallCheckBox_height;

        Vector2 r_topLeftPoint = transform.position;
        r_topLeftPoint.y += col.bounds.extents.y-wallCheckBox_height;

        Vector2 r_bottomRightPoint = transform.position;
        r_bottomRightPoint.x += col.bounds.extents.x;
        r_bottomRightPoint.y -= col.bounds.extents.y-wallCheckBox_height;

        bool onLeftWall = Physics2D.OverlapArea(l_topLeftPoint, l_bottomRightPoint, groundLayer);
        bool onRightWall = Physics2D.OverlapArea(r_topLeftPoint, r_bottomRightPoint, groundLayer);

        if (onLeftWall || onRightWall)
        {
            lastWall = Time.time;
        }

        return onLeftWall ? true : onRightWall ? true : false;
    }
}
