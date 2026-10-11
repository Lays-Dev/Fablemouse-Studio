
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    [Header("Movement Settings")]
    [SerializeField] private float _moveSpeed = 5f;
    [SerializeField] private float _sprintSpeed = 8f;

    [Header("Jump Settings")]
    [SerializeField] private float _jumpHeight = 5f;

    [Tooltip("Seconds between jumps.")]
    [SerializeField] private float _jumpCooldown = 1f;

    private Rigidbody _rigidbody;
    private PlayerInput _playerInput;

    private InputAction _moveAction;
    private InputAction _sprintAction;
    private InputAction _jumpAction;

    private bool _jumpRequested;
    private float _nextJumpTime;

    private void Awake()
    {
        _rigidbody = GetComponent<Rigidbody>();
        _playerInput = GetComponent<PlayerInput>();

        if (_rigidbody == null)
        {
            Debug.LogError("Player needs a Rigidbody.");
        }

        if (_playerInput == null)
        {
            Debug.LogError("Player needs a Player Input component.");
            return;
        }

        _moveAction = _playerInput.actions["Move"];
        _sprintAction = _playerInput.actions["Sprint"];
        _jumpAction = _playerInput.actions["Jump"];
    }

    private void Update()
    {
        // Only accept a jump press when the cooldown has finished.
        if (_jumpAction != null &&
            _jumpAction.WasPressedThisFrame() &&
            Time.time >= _nextJumpTime)
        {
            _jumpRequested = true;
            _nextJumpTime = Time.time + _jumpCooldown;
        }
    }

    private void FixedUpdate()
    {
        if (_rigidbody == null ||
            _moveAction == null ||
            _sprintAction == null)
        {
            return;
        }

        Vector2 moveInput = _moveAction.ReadValue<Vector2>();

        float currentSpeed = _moveSpeed;

        if (_sprintAction.IsPressed())
        {
            currentSpeed = _sprintSpeed;
        }

        Vector3 velocity = _rigidbody.linearVelocity;

        velocity.x = moveInput.x * currentSpeed;
        velocity.z = moveInput.y * currentSpeed;

        // Jump without checking whether the player is grounded.
        if (_jumpRequested)
        {
            velocity.y = Mathf.Sqrt(
                2f * Mathf.Abs(Physics.gravity.y) * _jumpHeight
            );

            _jumpRequested = false;
        }

        _rigidbody.linearVelocity = velocity;
    }
}
