using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    // Controls how fast the player moves
    [SerializeField] private float _moveSpeed = 5f;

    private Vector2 _moveInput;
    private Rigidbody _rigidbody;

    // Gets the player's movement input.
    public void OnMove(InputValue value)
    {
        _moveInput = value.Get<Vector2>();
    }

    //Moves the player left and right
    private void FixedUpdate()
    {
        if (_rigidbody == null)
        {
            return;
        }
        Vector3 velocity = _rigidbody.linearVelocity;
        velocity.x = _moveInput.x * _moveSpeed;
        _rigidbody.linearVelocity = velocity;
    }
    // Gets the Rigidbody attached to the player
    private void Awake()
    {
        _rigidbody = GetComponent<Rigidbody>();

        if (_rigidbody == null)
        {
            Debug.LogError("Player needs a Rigidbody.");
        }
    }
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
