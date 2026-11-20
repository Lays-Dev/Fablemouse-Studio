
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    [Header("Movement Settings")]
    [SerializeField] private float _moveSpeed = 3.5f;
    [SerializeField] private float _sprintSpeed = 8f;

    [Header("Jump Settings")]
    [SerializeField] private float _jumpHeight = 5f;

    [Tooltip("Seconds between jumps.")]
    [SerializeField] private float _jumpCooldown = 1f;

    [Header("Slope Settings")]
    [SerializeField] private float _maxSlopeAngle = 45f;

    private Rigidbody _rigidbody;
    private PlayerInput _playerInput;

    private InputAction _moveAction;
    private InputAction _sprintAction;
    private InputAction _jumpAction;

    private bool _jumpRequested;
    private float _nextJumpTime;

    private bool _playerFrozen;
    public bool IsFrozen => _playerFrozen;

    private RigidbodyConstraints _originalConstraints;

    // Reusable array for checking current collision contacts.
    private readonly ContactPoint[] _contacts = new ContactPoint[32];

    // Stores whether Lona is standing on walkable ground.
    private bool _isGrounded;

    // Stores whether Lona is touching a steep surface.
    private bool _touchingSteepSurface;

    // Direction pointing away from the steep surface.
    private Vector3 _steepSurfaceNormal;

    private void Awake()
    {
        _rigidbody = GetComponent<Rigidbody>();
        _playerInput = GetComponent<PlayerInput>();

        if (_rigidbody == null)
        {
            Debug.LogError("Player needs a Rigidbody.");
            enabled = false;
            return;
        }

        if (_playerInput == null)
        {
            Debug.LogError("Player needs a PlayerInput component.");
            enabled = false;
            return;
        }

        _moveAction = _playerInput.actions["Move"];
        _sprintAction = _playerInput.actions["Sprint"];
        _jumpAction = _playerInput.actions["Jump"];
    }

    public void FreezePlayer()
    {
        if (_playerFrozen)
            return;

        _playerFrozen = true;
        _jumpRequested = false;

        // Stop all movement.
        _rigidbody.linearVelocity = Vector3.zero;
        _rigidbody.angularVelocity = Vector3.zero;

        // Remember the original Rigidbody constraints.
        _originalConstraints = _rigidbody.constraints;

        // Prevent movement during dialogue.
        _rigidbody.constraints = _originalConstraints | RigidbodyConstraints.FreezePosition;

        Debug.Log("PLAYER FROZEN!");
    }

    public void UnfreezePlayer()
    {
        if (!_playerFrozen)
            return;

        // Restore the Rigidbody constraints.
        _rigidbody.constraints = _originalConstraints;

        _rigidbody.linearVelocity = Vector3.zero;
        _rigidbody.angularVelocity = Vector3.zero;

        _playerFrozen = false;
        Debug.Log("PLAYER UNFROZEN!");
    }

    private void Update()
    {
        if (_playerFrozen)
            return;

        // Only allow a jump when grounded and off cooldown.
        if (_jumpAction != null && _jumpAction.WasPressedThisFrame() && Time.time >= _nextJumpTime && _isGrounded)
        {
            _jumpRequested = true;
            _nextJumpTime = Time.time + _jumpCooldown;
        }
    }

    private void FixedUpdate()
    {
        if (_playerFrozen)
            return;

        if (_moveAction == null || _sprintAction == null)
            return;

        Vector2 moveInput = _moveAction.ReadValue<Vector2>();

        float currentSpeed = _moveSpeed;

        if (_sprintAction.IsPressed())
        {
            currentSpeed = _sprintSpeed;
        }

        Vector3 movement = new Vector3(
            moveInput.x * currentSpeed,
            0f,
            moveInput.y * currentSpeed
        );

        Vector3 velocity = _rigidbody.linearVelocity;

        if (_touchingSteepSurface)
        {
            float movementIntoSurface =
                Vector3.Dot(movement, -_steepSurfaceNormal);

            if (movementIntoSurface > 0f)
            {
                movement = Vector3.ProjectOnPlane(
                    movement,
                    _steepSurfaceNormal
                );

                movement.y = 0f;

                // Prevent climbing caused by collision velocity.
                if (_isGrounded && !_jumpRequested && velocity.y > 0f)
                {
                    velocity.y = 0f;
                }
            }
        }

        velocity.x = movement.x;
        velocity.z = movement.z;

        if (_jumpRequested)
        {
            velocity.y = Mathf.Sqrt(
                2f * Mathf.Abs(Physics.gravity.y) * _jumpHeight
            );

            _jumpRequested = false;
        }

        _rigidbody.linearVelocity = velocity;

        // Clear contact information after using it.
        CheckSurfaces();
    }
    private void OnCollisionStay(Collision collision)
    {
        int contactCount = collision.GetContacts(_contacts);

        for (int i = 0; i < contactCount; i++)
        {
            ContactPoint contact = _contacts[i];

            float surfaceAngle = Vector3.Angle(
                contact.normal,
                Vector3.up
            );

            if (surfaceAngle <= _maxSlopeAngle)
            {
                _isGrounded = true;
            }
            else
            {
                if (!_touchingSteepSurface ||
                    surfaceAngle >
                    Vector3.Angle(_steepSurfaceNormal, Vector3.up))
                {
                    _touchingSteepSurface = true;
                    _steepSurfaceNormal = contact.normal;
                }
            }
        }
    }
    private void OnCollisionEnter(Collision collision)
    {
        // Detect new contacts immediately.
        OnCollisionStay(collision);
    }
    private void CheckSurfaces()
    {
        _isGrounded = false;
        _touchingSteepSurface = false;
        _steepSurfaceNormal = Vector3.zero;
    }
}

