using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    // Controls player speed
    [SerializeField] private float _moveSpeed = 5f;

    // Controls player sprint speed
    [SerializeField] private float _sprintSpeed = 8f;

    private Rigidbody _rigidbody;
    private PlayerInput _playerInput;
    private InputAction _moveAction;
    private InputAction _sprintAction;

    // Grabs component and input actions to move the player.
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
    }

    // Moves the player left and right.
    private void FixedUpdate()
    {
        if (_rigidbody == null || _moveAction == null || _sprintAction == null)
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
        _rigidbody.linearVelocity = velocity;
    }
}