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
    private bool _isFacingRight=true;
    private bool _isGrounded;
    private bool _isJumping;
    private bool _jumpPressed;
    private bool _isActive = true;

    private float _lastGrounded;
    [SerializeField] private float _coyoteTime = 0.15f;
    private float _coyoteCooldownTimer;
    [SerializeField] private float _jumpTime = 0.15f;
    private float _jumpCooldownTime;

    [Space(10)]
    [Header("Movement variables")]
    [SerializeField] private bool _doConserveMomentum = true;
    private Vector2 _movementInput;
    [SerializeField] private float _moveSpeed = 9f;
    [SerializeField] private float _acceleration = 13f;
    [SerializeField] private float _decceleration = 16f;
    [SerializeField] private float _accelerationInAir = 0.65f;
    [SerializeField] private float _deccelerationInAir = 0.65f;
    [SerializeField] private float _velocityPower = 1f;
    [SerializeField] private float _frictionAmount = 1f;

    [Space(10)]
    [Header("Jump variables")]
    [SerializeField] private float _gravityScale = 2.5f;
    [SerializeField] private float _gravityScaleMult = 1.5f;
    [SerializeField] private float _fastFallGravityMult = 2f;
    [SerializeField] private float _jumpHangTimeThreshold = 1f;
    [SerializeField] private float _maxFastFallSpeed = 30f;
    [SerializeField] private float _maxFallSpeed = 25f;
    [SerializeField] private float _jumpForce = 16f;
    [SerializeField] private float _jumpHangAccelerationMult = 1.1f;
    [SerializeField] private float _jumpHangMaxSpeedMult = 1.3f;
    [SerializeField] private float _jumpHangThreshold = 1f;
    [SerializeField, Range(0, 1)] private float _jumpCutMult = 0.45f;

    [Space(10), Header("Respawn variables")]
    [SerializeField] private float _dieJumpAmount = 8f;
    [SerializeField] private float _respawnTime = 0.5f;
    [SerializeField] private Vector2 _respawnPosition;
    [SerializeField] private float _respawnJumpAmount = 12f;
    [SerializeField] private float _respawnCooldown = 0.1f;

    private void Awake()
    {
        _rigidBody = GetComponent<Rigidbody2D>();
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
        _respawnPosition = transform.position;
        _playerAction.Player.Jump.started += _ => JumpStarted();
        _playerAction.Player.Jump.canceled += _ => JumpCanceled();
    }
    private void FixedUpdate()
    {
        if (!_isActive)
        {
            return;
        }
        Move(1);
    }

    private void Update()
    {
        if (!_isActive)
        {
            return;
        }
        GravityCases();
        JumpCases();

        _isGrounded = _collision.IsGrounded();
        if (_isGrounded)
        {
            _coyoteCooldownTimer = _coyoteTime;
        }
        else
        {
            _coyoteCooldownTimer -= Time.deltaTime;
        }

        if (_jumpPressed)
        {
            _jumpPressed = false;
            _jumpCooldownTime = _jumpTime;
        }
        else
        {
            _jumpCooldownTime -= Time.deltaTime;
        }

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
        Debug.Log("jump started");
        if (!_isActive)
        {
            return;
        }
        _jumpPressed = true;
        _lastGrounded = _collision._lastGrounded;
        Jump();
    }

    private void Jump()
    {
        if (_coyoteCooldownTimer > 0f)
        {
            Debug.Log("jumping");
            _isJumping = true;
            _rigidBody.AddForce(Vector2.up * _jumpForce, ForceMode2D.Impulse);
        }
    }

    private void JumpCanceled()
    {
        Debug.Log("jump canceled");
        if (_isJumping)
        {
            _rigidBody.AddForce(Vector2.down * _rigidBody.velocity.y * (1 - _jumpCutMult), ForceMode2D.Impulse);
        }
        _isJumping = false;
        _coyoteCooldownTimer = 0f;
    }

    private void JumpCases()
    {
        if (!_isJumping && _jumpCooldownTime > 0f && _isGrounded)
        {
            Debug.Log("jump case");
            _isJumping = true;
            _rigidBody.AddForce(Vector2.up * _jumpForce, ForceMode2D.Impulse);
            _jumpCooldownTime = 0f;
        }
    }
    private void MiniJump(float _jumpPower)
    {
        _rigidBody.AddForce(Vector2.up * _jumpPower, ForceMode2D.Impulse);
    }

    private void GravityCases()
    {
        if (_rigidBody.velocity.y < 0 && (_movementInput.y < 0) || (!_isJumping && _jumpCooldownTime > 0f && _isGrounded))
        {
            SetGravityScale(_gravityScale * _fastFallGravityMult);
            _rigidBody.velocity = new Vector2(_rigidBody.velocity.x, Mathf.Max(_rigidBody.velocity.y, -_maxFastFallSpeed));
        }
        else if (_isJumping && Mathf.Abs(_rigidBody.velocity.y) < _jumpHangTimeThreshold)
        {
            SetGravityScale(_gravityScale * _jumpHangTimeThreshold);
        }
        else if (_rigidBody.velocity.y < 0)
        {
            SetGravityScale(_gravityScale * _gravityScaleMult);
            _rigidBody.velocity = new Vector2(_rigidBody.velocity.x, Mathf.Max(_rigidBody.velocity.y, -_maxFallSpeed));
        }
        else
        {
            SetGravityScale(_gravityScale);
        }
    }

    public void SetGravityScale(float scale)
    {
        _rigidBody.gravityScale = scale;
    }
    private void Flip()
    {
        _isFacingRight = !_isFacingRight;
        Vector3 _localScale = transform.localScale;
        _localScale.x *= -1f;
        transform.localScale = _localScale;
    }

    public void Die()
    {
        _isActive = false;
        MiniJump(_dieJumpAmount);
        SetGravityScale(_gravityScale);
        _rigidBody.velocity = new Vector2(0, _rigidBody.velocity.y);
        StartCoroutine(Respawn(_respawnTime));
    }

    public void SetRespawnPoint(Vector2 _position)
    {
        _respawnPosition = _position;
    }

    private IEnumerator Respawn(float _respawnTime)
    {
        yield return new WaitForSeconds(_respawnTime);
        transform.position = _respawnPosition;
        MiniJump(_respawnJumpAmount);
        StartCoroutine(RespawnCooldown(_respawnCooldown));
    }

    private IEnumerator RespawnCooldown(float _respawnCooldown)
    {
        yield return new WaitForSeconds(_respawnCooldown);
        _isActive = true;
    }
}
