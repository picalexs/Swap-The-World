using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UIElements;

public class PlayerScript : MonoBehaviour
{
    private PlayerActionControler _moveAction;
    private Rigidbody2D _rigidBody;
    private CapsuleCollider2D _capsuleCollider;

    [Space(10)]
    private bool _isFacingRight;
    private bool _isGrounded;
    private bool _isJumping;

    private float _lastGrounded;

    [Space(10)]
    [Header("Movement variables")]
    private bool _doConserveMomentum = true;
    private Vector2 _movementInput;
    [SerializeField] private float _moveSpeed;
    [SerializeField] private float _acceleration;
    [SerializeField] private float _decceleration;
    [SerializeField] private float _accelerationInAir;
    [SerializeField] private float _deccelerationInAir;
    [SerializeField] private float _velocityPower;
    [SerializeField] private float _frictionAmount;

    [Space(10)]
    [Header("Jump variables")]
    [SerializeField] private float _jumpForce;
    [SerializeField] private float _jumpHangAccelerationMult;
    [SerializeField] private float _jumpHangMaxSpeedMult;
    [SerializeField] private float _jumpHangThreshold;



    private void Awake()
    {
        _rigidBody=GetComponent<Rigidbody2D>();
        _capsuleCollider=GetComponent<CapsuleCollider2D>();
        _moveAction = new PlayerActionControler();
    }
private void OnEnable()
    {
        _moveAction.Enable();
    }

    private void OnDisable()
    {
        _moveAction.Disable();
    }
    private void FixedUpdate()
    {
        Move(1);
    }

    private void Move(float _lerpAmount)
    {
        _movementInput = _moveAction.Player.Move.ReadValue<Vector2>();
        float _targetSpeed = _movementInput.x * _moveSpeed;
        _targetSpeed = Mathf.Lerp(_rigidBody.velocity.x, _targetSpeed, _lerpAmount);

        float _accelRate;
        if (Time.time - _lastGrounded > 0)
            _accelRate = (Mathf.Abs(_targetSpeed) > 0.01f) ? _acceleration : _decceleration;
        else
            _accelRate = (Mathf.Abs(_targetSpeed) > 0.01f) ? _acceleration * _accelerationInAir : _decceleration * _deccelerationInAir;

        if ((_isJumping) && Mathf.Abs(_rigidBody.velocity.y) < _jumpHangThreshold)
        {
            _accelRate *= _jumpHangAccelerationMult;
            _targetSpeed *= _jumpHangMaxSpeedMult;
        }

        if (_doConserveMomentum && Mathf.Abs(_rigidBody.velocity.x) > Mathf.Abs(_targetSpeed)
            && Mathf.Sign(_rigidBody.velocity.x) == Mathf.Sign(_targetSpeed)
            && Mathf.Abs(_targetSpeed) > 0.01f && Time.time - _lastGrounded < 0)
        {
            _accelRate = 0;
        }

        float speedDif = _targetSpeed - _rigidBody.velocity.x;
        float movement = Mathf.Pow(Mathf.Abs(speedDif) * _accelRate, _velocityPower) * Mathf.Sign(speedDif);
        _rigidBody.AddForce(movement * Vector2.right);

        if (_isGrounded && Mathf.Abs(_movementInput.x) < 0.01f)
        {
            float amount = Mathf.Min(Mathf.Abs(_rigidBody.velocity.x), Mathf.Abs(_frictionAmount));
            amount *= Mathf.Sign(_rigidBody.velocity.x);
            _rigidBody.AddForce(Vector2.right * -amount, ForceMode2D.Impulse);
        }
    }

    private void Flip()
    {
        _isFacingRight = !_isFacingRight;
        Vector3 _localScale = transform.localScale;
        _localScale.x *= -1f;
        transform.localScale = _localScale;
    }

}
