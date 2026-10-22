using UnityEngine;
using UnityEngine.InputSystem;

public class Live2DAnimationController : MonoBehaviour
{
    public Animator animator;

    private PlayerInput playerInput;
    private InputAction moveAction;
    private InputAction sprintAction;
    private PlayerMovement movement;

    // remembers which way she was going when she stops
    private bool isFront = false;

    private void Awake()
    {
        playerInput = GetComponent<PlayerInput>();
        moveAction = playerInput.actions["Move"];
        sprintAction = playerInput.actions["Sprint"];
        movement = GetComponent<PlayerMovement>();
    }

    private void Update()
    {
        float horizontalInput = moveAction.ReadValue<Vector2>().x;

        animator.SetFloat("Speed", Mathf.Abs(horizontalInput));

        // which way is she going? toward/away from the camera = front, left/right = side
        Vector2 input = moveAction.ReadValue<Vector2>();
        bool moving = input.magnitude > 0.1f;
        if (moving) isFront = Mathf.Abs(input.y) > Mathf.Abs(input.x);

        // zoomies, only if she's actually moving
        bool running = moving && sprintAction.IsPressed();

        animator.SetBool("IsRunning", running);
        animator.SetBool("IsFront", isFront);
    }

    // Player Input calls this by itself when interact (E) is pressed
    private void OnInteract()
    {
        // no interacting while paused or in a mini game
        if (movement != null && !movement.enabled) return;

        animator.SetTrigger("Interact");
    }
}