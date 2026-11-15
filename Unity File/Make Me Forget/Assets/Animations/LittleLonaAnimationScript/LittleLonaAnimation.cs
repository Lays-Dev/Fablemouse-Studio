using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections;
// Using Input System because this script uses PlayerInput and InputAction.

// NEW: Used for the talking animation.
using TMPro;
using Yarn.Unity;

// This script will control Little Lona's walk left and right animation.
public class Live2DAnimationController : MonoBehaviour
{

#region Inspector

    [Header("Animation")]
    [Tooltip("Drag lil lona live2D Animator component here.")]
    public Animator animator;

    [Header("Character Scale")]
    [Tooltip("Current scale of the character.")]
    [SerializeField] private float facingScale = 3f;

    [Header("Interaction")]
    [Tooltip("How long the interaction animation plays.")]
    // Estimated to be around 0.9 sec long.
    [SerializeField] private float interactionDuration = 0.9f;

    // NEW
    [Header("Talking")]
    [Tooltip("Drag the Dialogue System here.")]
    public DialogueRunner dialogueRunner;

    [Tooltip("Dialogue System > Canvas > Line Presenter > Character Name.")]
    public TMP_Text characterNameText;

    [Tooltip("Names in the Yarn file that count as Lona talking.")]
    public string[] lonaNames = { "Lona", "Little Lona" };


#endregion
#region Variables

    // Stores references to the new input system components. 
    // These components must be attached to the same GameObject as this script.
    private PlayerInput playerInput;
    private InputAction moveAction;
    private InputAction sprintAction;
    private InputAction interactAction;

    // Holds the players direction. 1 being Right. -1 being Left.
    // This will keep the script from updating Little Lona's scale every frame.
    // Instead, check if the direction changed first.
    private int currentDirection = 1;

    // Is the player pressing the interact key.
    // This will be used for the interact animation.
    private bool isInteracting = false;

    // NEW: Used to check if the player is frozen (cutscenes and dialogue).
    private PlayerMovement playerMovement;

    // NEW: Remembers if the player was frozen last frame.
    private bool wasFrozen = false;

    // NEW: Is she facing the camera (last moved towards it).
    // Used to pick the front or side interact animation.
    private bool facingFront = false;

    // NEW: Remembers if she was talking last frame.
    private bool wasTalking = false;

#endregion
#region Awake

    // Find and store references to components when the script is initialized.
    private void Awake()
    {
        // NEW: Get the PlayerMovement component on the same GameObject.
        playerMovement = GetComponent<PlayerMovement>();

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

        // Get the interact action
        interactAction = playerInput.actions["Interact"];

        // Null check
        if (interactAction == null)
        {
            Debug.LogError("Interact action was not found in PlayerInput.", this);
            return;
        }
    }

#endregion
#region Update

    private void Update()
    {
        // Null check.
        if (moveAction == null || sprintAction == null || interactAction == null)
        {
            Debug.LogError("Move, Sprint, or Interact action was not found in PlayerInput.", this);
            return;
        }

        // NEW: Talking animation. Checked before the frozen check,
        // because she is usually frozen while dialogue plays.
        UpdateTalking();

        // NEW: Ignore input while the player is frozen.
        // This lets cutscenes control the animation instead.
        if (playerMovement != null && playerMovement.IsFrozen)
        {
            // Only stop the animations once, right when she gets frozen.
            if (!wasFrozen)
            {
                animator.SetFloat("Speed", 0f);
                animator.SetBool("IsSprinting", false);
                animator.SetBool("IsMovingFront", false);
                wasFrozen = true;
            }

            return;
        }

        wasFrozen = false;


        // Checks if the interact button was pressed and the player is not currently interacting.
        if (interactAction.WasPressedThisFrame() && !isInteracting)
        {
            // Debug.Log("Interact action was pressed!");
            // StartCoroutine tells the script to wait for the animation to play before continuing. 
            StartCoroutine(PlayInteraction());
        }

        // Prevents the other animations from interupting the Interact animation.
        if (isInteracting)
        {
            return;
        }

        //Reads the current value of the "Move" Input Action.
        // Using the X value.
        Vector2 moveInput = moveAction.ReadValue<Vector2>();

        // NEW: Moving towards the camera (negative Z) more than sideways.
        bool movingFront = moveInput.y < -0.01f && Mathf.Abs(moveInput.y) > Mathf.Abs(moveInput.x);

        // NEW: Remember which way she's facing for the interact animation.
        if (movingFront)
        {
            facingFront = true;
        }
        else if (Mathf.Abs(moveInput.x) > 0.01f)
        {
            facingFront = false;
        }

        // Checks if the player is sprinting by checking the sprint action keybind
        // NEW: Also used for Walk_Front vs Sprint_Front.
        bool isSprinting = sprintAction.IsPressed() && moveInput.magnitude > 0.01f;

        // Update animation speed.
        // NEW: Speed stays 0 while running front, so the side walk doesn't play.
        animator.SetFloat("Speed", movingFront ? 0f : Mathf.Abs(moveInput.x));

        animator.SetBool("IsSprinting", isSprinting);

        // NEW: Front run and facing direction.
        animator.SetBool("IsMovingFront", movingFront);
        animator.SetBool("IsFacingFront", facingFront);

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

#endregion
#region Interaction

    private IEnumerator PlayInteraction()
    {
        // Lock Interaction so it cannot be triggered repeatedly.
        isInteracting = true;

        // Stop Walking and Sprinting animations.
        animator.SetFloat("Speed", 0f);
        animator.SetBool("IsSprinting", false);
        animator.SetBool("IsMovingFront", false);

        // NEW: Picks front or side interact in the Animator.
        animator.SetBool("IsFacingFront", facingFront);

        // Set Interaction bool to true.
        animator.SetBool("isInteracting", true);

        // Wait for the Interaction animation to finish.
        yield return new WaitForSeconds(interactionDuration);

        // Set Interaction bool to false.
        animator.SetBool("isInteracting", false);

        // Allow movement animations again.
        isInteracting = false;
    }

#endregion
#region Talking

    // NEW: Plays the talking animation while one of Lona's lines is showing.
    private void UpdateTalking()
    {
        if (dialogueRunner == null || characterNameText == null) return;

        bool talking = dialogueRunner.IsDialogueRunning
            && characterNameText.gameObject.activeInHierarchy
            && System.Array.IndexOf(lonaNames, characterNameText.text) >= 0;

        // Only tell the Animator when it changes.
        if (talking != wasTalking)
        {
            animator.SetBool("IsTalking", talking);
            wasTalking = talking;
        }
    }

#endregion
#region Walking Left

    // NEW: Lets cutscenes turn her left (-1) or right (1).
    public void FaceDirection(int direction)
    {
        facingFront = false;

        if (direction == 0 || direction == currentDirection) return;

        currentDirection = direction;
        UpdateFacingDirection();
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

#endregion

}