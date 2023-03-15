using System.Collections;
using System.ComponentModel;
using UnityEngine;


public class PlayerScript : MonoBehaviour
{
    [SerializeField,Description("Player")] private GameObject playerObject;
    private PlayerActionControler _playerAction;
    private Rigidbody2D _rigidBody;
    private Renderer playerRenderer;
    [SerializeField] private LayerMask _groundLayer;


    [Space(10)]
    private bool _isFacingRight=true;
    private bool _isGrounded;
    private bool _isJumping;
    private bool _jumpPressed;
    private bool _isActive = true;
    private bool _isSwaped = false;
    private bool _isPressing;

    private float _lastGrounded;
    [SerializeField] private float _pressedTime;
    [SerializeField] private float _coyoteTime = 0.15f;
    [SerializeField] private float _coyoteCooldownTimer;
    [SerializeField] private float _jumpTime = 0.15f;
    [SerializeField] private float _jumpCooldownTime;
    [SerializeField] private Vector2 _groundCheckSize;

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
        _playerAction = new PlayerActionControler();
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
        _rigidBody = playerObject.GetComponent<Rigidbody2D>();
        playerRenderer = playerObject.GetComponent<Renderer>();
        _respawnPosition = playerObject.transform.position;
        _playerAction.Player.Jump.started += _ => JumpStarted();
        _playerAction.Player.Jump.canceled += _ => JumpCanceled();
    }

    public void ChangePlayerObjectTo(GameObject newObject)
    {
        _isSwaped = !_isSwaped;
        playerObject = newObject;
        _rigidBody = playerObject.GetComponent<Rigidbody2D>();
        playerRenderer = playerObject.GetComponent<Renderer>();
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
        IsGrounded();

        if (_isSwaped)
        {
            return;
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
        _isPressing = true;
        _pressedTime = 0;
        Jump();
    }

    private void Jump()
    {
        if (_coyoteCooldownTimer > 0f && !_isJumping)
        {
            Debug.Log("jumping");
            _isJumping = true;
            _coyoteCooldownTimer = 0f;
            _rigidBody.AddForce(Vector2.up * _jumpForce, ForceMode2D.Impulse);
        }
    }

    private void JumpCanceled()
    {
        _isPressing = false;
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

        if (_isPressing)
        {
            _pressedTime += Time.deltaTime;
        } else
        {
            _pressedTime -= Time.deltaTime;
        }
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

        if (!_isJumping && _isGrounded && _jumpCooldownTime > 0f)
        {
            if (_pressedTime > 0f)
            {
                Debug.Log("jump case");
                _jumpCooldownTime = 0f;
                _isJumping = true;
                _rigidBody.AddForce(Vector2.up * _jumpForce, ForceMode2D.Impulse);
            }
            else
            {
                _rigidBody.AddForce(Vector2.down * _rigidBody.velocity.y * (1 - _jumpCutMult), ForceMode2D.Impulse);
                _isJumping = false;
            }
        }
       
    }
    private void MiniJump(float _jumpPower)
    {
        _rigidBody.AddForce(Vector2.up * _jumpPower, ForceMode2D.Impulse);
    }

    private void GravityCases()
    {
        if (_rigidBody.velocity.y < 0 && (_movementInput.y < 0))
        {
            SetGravityScale(_gravityScale * _fastFallGravityMult);
            _rigidBody.velocity = new Vector2(_rigidBody.velocity.x, Mathf.Max(_rigidBody.velocity.y, -_maxFastFallSpeed));
        }
        else if (_isJumping && Mathf.Abs(_rigidBody.velocity.y) < _jumpHangTimeThreshold)
        {
            SetGravityScale(_gravityScale * _jumpHangTimeThreshold);
        }
        else if (_rigidBody.velocity.y < 0 && _coyoteTime < 0)
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
        Vector3 _localScale = playerObject.transform.localScale;
        _localScale.x *= -1f;
        playerObject.transform.localScale = _localScale;
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
        playerObject.transform.position = _respawnPosition;
        MiniJump(_respawnJumpAmount);
        StartCoroutine(RespawnCooldown(_respawnCooldown));
    }

    private IEnumerator RespawnCooldown(float _respawnCooldown)
    {
        yield return new WaitForSeconds(_respawnCooldown);
        _isActive = true;
    }
    public void IsGrounded()
    {
        Vector3 lowestPosition = new Vector3(playerRenderer.bounds.center.x, playerRenderer.bounds.min.y, 0f);
        _isGrounded = Physics2D.OverlapBox(lowestPosition, _groundCheckSize, 0, _groundLayer);
        if (_isGrounded)
        {
            _lastGrounded = Time.time;
        }
    }
}
