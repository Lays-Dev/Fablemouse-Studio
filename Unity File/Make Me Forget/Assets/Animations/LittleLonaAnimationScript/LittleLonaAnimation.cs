using UnityEngine;
using UnityEngine.InputSystem;
// Using Input System because this script uses PlayerInput and InputAction.

// This script will control Little Lona's walk left and right animation.
public class Live2DAnimationController : MonoBehaviour
{
    [Header("Animation")]
    [Tooltip("Drag lil lona live2D Animator component here.")]
    public Animator animator;

    [Header("Character Scale")]
    [Tooltip("Current scale of the character.")]
    [SerializeField] private float facingScale = 3f;

    // Stores references to the new input system components. 
    // These components must be attached to the same GameObject as this script.
    private PlayerInput playerInput;
    private InputAction moveAction;
    private InputAction sprintAction;

    // Holds the players direction. 1 being Right. -1 being Left.
    // This will keep the script from updating Little Lona's scale every frame.
    // Instead, check if the direction changed first.
    private int currentDirection = 1;

    // Find and store references to components when the script is initialized.
    private void Awake()
    {
        // Get the PlayerInput component. This must be on the same GameObject as this script.
        playerInput = GetComponent<PlayerInput>();

        // Null check.
        if (playerInput == null)
        {
            Debug.LogError("PlayerInput component is missing.", this);
            return;
        }

        // Get the move action.
        moveAction = playerInput.actions["Move"];

        // Null check.
        if (moveAction == null)
        {
            Debug.LogError("Move action was not found in PlayerInput.", this);
            return;
        }

        // Get the sprint action
        sprintAction = playerInput.actions["Sprint"];

        // Null check
        if (sprintAction == null)
        {
            Debug.LogError("Sprint action was not found in PlayerInput.", this);
            return;
        }
    }

    private void Update()
    {
        // Null check.
        if (moveAction == null || sprintAction == null)
        {
            Debug.LogError("Move or Sprint action was not found in PlayerInput.", this);
            return;
        }

        //Reads the current value of the "Move" Input Action.
        // Using the X value.
        Vector2 moveInput = moveAction.ReadValue<Vector2>();

        // Checks if the player is sprinting by checking the sprint action keybind
        bool isSprinting = sprintAction.IsPressed() && moveInput.magnitude > 0.01f;

        // Update animation speed.
        animator.SetFloat("Speed", Mathf.Abs(moveInput.x));

        animator.SetBool("IsSprinting", isSprinting);

        // Flip immediately when direction changes.
        if (moveInput.x < 0 && currentDirection != -1)
        {
            currentDirection = -1;
            UpdateFacingDirection();
        }
        else if (moveInput.x > 0 && currentDirection != 1)
        {
            currentDirection = 1;
            UpdateFacingDirection();
        }
    }

    // Update the characters scale.
    // This will make it appear as if Little Lona is moving left.
    private void UpdateFacingDirection()
    {
        transform.localScale = new Vector3(
            facingScale * currentDirection,
            transform.localScale.y,
            transform.localScale.z
        );
    }
}
