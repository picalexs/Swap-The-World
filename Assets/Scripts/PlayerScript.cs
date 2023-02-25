using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UIElements;

public class PlayerScript : MonoBehaviour
{
    private PlayerActionControler _playerAction;
    private Rigidbody2D _rigidBody;
    private CollisionCheck _collision;

    [Space(10)]
    private bool _isFacingRight;
    private bool _isGrounded;
    private bool _isJumping;

    private float _lastGrounded;

    [Space(10)]
    [Header("Movement variables")]
    [SerializeField] private bool _doConserveMomentum = true;
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
    [SerializeField] private float _coyoteCooldown = 0.15f;
    [SerializeField] private float _jumpForce;
    [SerializeField] private float _jumpHangAccelerationMult;
    [SerializeField] private float _jumpHangMaxSpeedMult;
    [SerializeField] private float _jumpHangThreshold;
    [SerializeField, Range(0,1)] private float _jumpCutMult;

    private void Awake()
    {
        _rigidBody=GetComponent<Rigidbody2D>();
        _playerAction = new PlayerActionControler();
        _collision = GetComponent<CollisionCheck>();
    }
    private void OnEnable()
    {
        _playerAction.Enable();
    }

    private void OnDisable()
    {
        _playerAction.Disable();
    }

    private void Start()
    {
        _playerAction.Player.Jump.started += _ => JumpStarted();
        _playerAction.Player.Jump.canceled += _ => JumpCanceled();
    }
    private void FixedUpdate()
    {
        Move(1);
    }

    private void Update()
    {
        _isGrounded = _collision.IsGrounded();
        if (!_isFacingRight && _movementInput.x > 0f)
        {
            Flip();
        }
        else if (_isFacingRight && _movementInput.x < 0f)
        {
            Flip();
        }
    }
    private void Move(float _lerpAmount)
    {
        _movementInput = _playerAction.Player.Move.ReadValue<Vector2>();
        float _targetSpeed = _movementInput.x * _moveSpeed;
        _targetSpeed = Mathf.Lerp(_rigidBody.velocity.x, _targetSpeed, _lerpAmount);

        float _accelRate;
        if (Time.time - _lastGrounded > 0)
        {
            _accelRate = (Mathf.Abs(_targetSpeed) > 0.01f) ? _acceleration : _decceleration;
        }
        else
        {
            _accelRate = (Mathf.Abs(_targetSpeed) > 0.01f) ? _acceleration * _accelerationInAir : _decceleration * _deccelerationInAir;
        }

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

    private void JumpStarted()
    {
        _lastGrounded = _collision._lastGrounded;
        Jump();
    }

    private void Jump()
    {
        if (_isGrounded || Time.time - _lastGrounded <= _coyoteCooldown)
        {
            _isJumping = true;
            _rigidBody.AddForce(Vector2.up * _jumpForce, ForceMode2D.Impulse);
        }
    }

    private void JumpCanceled()
    {
        if (_isJumping)
        {
            _rigidBody.AddForce(Vector2.down * _rigidBody.velocity.y * (1 - _jumpCutMult), ForceMode2D.Impulse);
        }
        _isJumping = false;
    }

    private void Flip()
    {
        _isFacingRight = !_isFacingRight;
        Vector3 _localScale = transform.localScale;
        _localScale.x *= -1f;
        transform.localScale = _localScale;
    }

}
