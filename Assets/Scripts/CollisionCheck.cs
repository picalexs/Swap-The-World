using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollisionCheck : MonoBehaviour
{
    [SerializeField] private Transform _groundCheck;
    [SerializeField] private Vector2 _groundCheckSize;
    [SerializeField] private LayerMask _groundLayer;
    [SerializeField] private float _lastGrounded;

    private void Awake()
    {
        _groundCheck = GetComponentInChildren<Transform>();
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
