using System.Collections;
using System.ComponentModel;
using UnityEngine;


public class PlayerScript : MonoBehaviour
{
    [SerializeField, Description("Player")] private GameObject playerObject;
    [SerializeField] private Material _playerMaterial;
    private PlayerActionControler _playerAction;
    private Rigidbody2D _rigidBody;
    private Renderer _playerRenderer;
    [SerializeField] private LayerMask _groundLayer;
    private Animator _anim;
    [SerializeField] private GameObject _transitionManager;
    private Animator _transition;

    [Space(10), Header("Sound")]
    [SerializeField] private AudioSource _jumpSound;
    [SerializeField] private AudioSource[] _dirtSound;
    [SerializeField] private AudioSource _deathSound;
    [SerializeField] private float _playSoundCooldown = 0.1f;
    private float _playSoundTime = 0f;

    [Space(10), Header("Booleans")]
    private bool _isFacingRight = true;
    [SerializeField] private bool _isGrounded;
    private bool _isJumping;
    private bool _jumpPressed;
    public static bool _isActive = true;
    public bool _isSwapped = false;
    public bool _isSwappedPropriety = false;
    private bool _isPressing;
    public static bool _isRunning;
    public bool _canMove = true;
    public bool _canJump = true;

    [Space(10), Header("Jump variables")]
    private float _lastGrounded;
    [SerializeField] private float _pressedTime;
    [SerializeField] private float _coyoteTime = 0.15f;
    [SerializeField] private float _coyoteCooldownTimer;
    [SerializeField] private float _jumpTime = 0.15f;
    [SerializeField] private float _jumpCooldownTime;
    [SerializeField] private Vector2 _groundCheckSize;
    [SerializeField] private Vector2 _boxGroundCheckSize;

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
    [SerializeField] private Color hitColor;
    [SerializeField] private float hitGlowAmount;
    [SerializeField] private float _dieJumpAmount = 8f;
    [SerializeField] private float _respawnTime = 0.5f;
    [SerializeField] public Vector2 _respawnPosition;
    [SerializeField] private float _respawnJumpAmount = 3f;
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
        _playerMaterial = playerObject.GetComponent<Renderer>().material;
        _anim = playerObject.GetComponent<Animator>();
        _rigidBody = playerObject.GetComponent<Rigidbody2D>();
        _playerRenderer = playerObject.GetComponent<Renderer>();
        _respawnPosition = playerObject.transform.position;
        _playerAction.Player.Jump.started += _ => JumpStarted();
        _playerAction.Player.Jump.canceled += _ => JumpCanceled();

        _transition = _transitionManager.GetComponentInChildren<Animator>();
    }

    public void ChangePlayerObjectTo(GameObject newObject)
    {
        _isSwapped = !_isSwapped;
        Debug.Log("isSwaped:" + _isSwapped);
        _isRunning = false;
        playerObject = newObject;
        _rigidBody = playerObject.GetComponent<Rigidbody2D>();
        _playerRenderer = playerObject.GetComponent<Renderer>();
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
            float fadeSpeed = 10f;
            _playerMaterial.SetColor("_HitEffectColor", hitColor);
            _playerMaterial.SetFloat("_HitEffectGlow", hitGlowAmount);
            _playerMaterial.SetFloat("_HitEffectBlend", Mathf.Lerp(_playerMaterial.GetFloat("_HitEffectBlend"), 0f, Time.deltaTime * fadeSpeed));
            return;
        }
        GravityCases();
        JumpCases();
        IsGrounded();

        if (!_isSwapped)
        {
            if (!_isFacingRight && _movementInput.x > 0f)
            {
                Flip();
            }
            else if (_isFacingRight && _movementInput.x < 0f)
            {
                Flip();
            }
        }
    }
    private void Move(float _lerpAmount)
    {
        if (_canMove == false)
        {
            return;
        }
        _movementInput = _playerAction.Player.Move.ReadValue<Vector2>();
        if (!_isSwapped)
        {
            if (_movementInput.x == 0)
            {
                _isRunning = false;
            }
            else
            {
                _isRunning = true;
            }
        }
        _anim.SetBool("isRunning", _isRunning);

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

        _playSoundTime -= Time.deltaTime;
        if (_isGrounded && Mathf.Abs(_movementInput.x) > 0)
        {
            if (_playSoundTime < 0f)
            {
                int randomIndex = Random.Range(0, _dirtSound.Length);
                _dirtSound[randomIndex].Play();
                _playSoundTime = _playSoundCooldown;
            }
        }

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
        if (_canJump == false)
        {
            return;
        }
        if (_coyoteCooldownTimer > 0f && !_isJumping)
        {
            _jumpSound.Play();
            Debug.Log("jumping");
            _isPressing = false;
            _pressedTime = 0f;
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
            _rigidBody.AddForce((1 - _jumpCutMult) * _rigidBody.velocity.y * Vector2.down, ForceMode2D.Impulse);
        }
        _isJumping = false;
        _coyoteCooldownTimer = 0f;
        _jumpCooldownTime = 0f;
    }

    private void JumpCases()
    {
        if (_canJump == false)
        {
            return;
        }

        if (_isPressing)
        {
            _pressedTime += Time.deltaTime;
        }
        else
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
                _jumpSound.Play();
                _rigidBody.AddForce(Vector2.up * _jumpForce, ForceMode2D.Impulse);
            }
            else
            {
                _rigidBody.AddForce((1 - _jumpCutMult) * _rigidBody.velocity.y * Vector2.down, ForceMode2D.Impulse);
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
        if (_isSwappedPropriety)
        {
            return;
        }
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
        if (!_isActive)
        {
            return;
        }

        _pressedTime = 0f;
        _jumpCooldownTime = 0f;
        _isActive = false;
        _isJumping = false;
        SetGravityScale(_gravityScale);
        _rigidBody.velocity = new Vector2(0, _rigidBody.velocity.y);

        _deathSound.Play();
        _playerMaterial.SetFloat("_HitEffectBlend", 1f);
        _transition.SetBool("Start", true);
        MiniJump(_dieJumpAmount);
        StartCoroutine(Respawn(_respawnTime));
    }

    public void SetRespawnPoint(Vector2 _position)
    {
        _respawnPosition = _position + new Vector2(0, 1f);
    }

    private IEnumerator Respawn(float _respawnTime)
    {
        yield return new WaitForSeconds(_respawnTime);
        _rigidBody.velocity = new Vector2(0, 0);
        playerObject.transform.position = _respawnPosition;
        _playerMaterial.SetFloat("_HitEffectBlend", 0f);
        MiniJump(_respawnJumpAmount);
        StartCoroutine(RespawnCooldown(_respawnCooldown));
    }

    private IEnumerator RespawnCooldown(float _respawnCooldown)
    {
        yield return new WaitForSeconds(_respawnCooldown);
        _isActive = true;
        _transition.SetBool("Start", false);
    }
    private void IsGrounded()
    {
        
        Collider2D[] colliders;
        if (_isSwapped)
        {
            Vector3 lowestPosition = new(_playerRenderer.bounds.center.x, _playerRenderer.bounds.center.y - 1f, 0f);
            colliders = Physics2D.OverlapBoxAll(lowestPosition, _boxGroundCheckSize, 0, _groundLayer);
        }
        else
        {
            Vector3 lowestPosition = new(_playerRenderer.bounds.center.x, _playerRenderer.bounds.min.y, 0f);
            colliders = Physics2D.OverlapBoxAll(lowestPosition, _groundCheckSize, 0, _groundLayer);
        }
        _isGrounded = false;
        foreach (Collider2D collider in colliders)
        {
            if (collider.gameObject != playerObject)
            {
                _isGrounded = true;
                break;
            }
            else
            {
                _isGrounded = false;
            }
        }
        if (_isGrounded)
        {
            _lastGrounded = Time.time;
        }
    }
}
