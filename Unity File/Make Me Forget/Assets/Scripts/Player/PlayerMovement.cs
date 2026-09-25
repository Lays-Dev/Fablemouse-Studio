using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    // Controls how fast the player moves.
    [SerializeField] private float _moveSpeed = 5f;

    // Controls how fast the player moves while sprinting.
    [SerializeField] private float _sprintSpeed = 8f;

    private Vector2 _moveInput;
    private bool _isSprinting;
    private Rigidbody _rigidbody;

    // Gets the Rigidbody attached to the player.
    private void Awake()
    {
        _rigidbody = GetComponent<Rigidbody>();

        if (_rigidbody == null)
        {
            Debug.LogError("Player needs a Rigidbody.");
        }
    }

    // Moves the player left and right.
    private void FixedUpdate()
    {
        if (_rigidbody == null)
        {
            return;
        }

        float currentSpeed = _moveSpeed;

        if (_isSprinting)
        {
            currentSpeed = _sprintSpeed;
        }

        Vector3 velocity = _rigidbody.linearVelocity;

        velocity.x = _moveInput.x * currentSpeed;

        _rigidbody.linearVelocity = velocity;
    }

    // Gets the player's movement input.
    public void OnMove(InputValue value)
    {
        _moveInput = value.Get<Vector2>();
    }

    // Gets the player's sprint input.
    public void OnSprint(InputValue value)
    {
        _isSprinting = value.isPressed;
    }
}
