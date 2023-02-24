using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollisionCheck : MonoBehaviour
{
    [SerializeField] private Transform _groundCheck;
    [SerializeField] private Vector2 _groundCheckSize;
    [SerializeField] private LayerMask _groundLayer;
    public float _lastGrounded;

    private void Awake()
    {
        _groundCheck = GetComponentsInChildren<Transform>()[1]; //gets the second transform it finds from the parent + childrens
    }
    public bool IsGrounded()
    {
        bool grounded = Physics2D.OverlapBox(_groundCheck.position, _groundCheckSize, 0, _groundLayer);
        if (grounded)
        {
            _lastGrounded = Time.time;
        }
        return grounded;
    }
}
