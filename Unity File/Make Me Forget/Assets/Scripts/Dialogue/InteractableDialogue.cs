using UnityEngine;
using UnityEngine.InputSystem;
using Yarn.Unity;

public class InteractableDialogue : MonoBehaviour
{
    // Stores the Yarn dialogue nodes this character can use.
    [SerializeField] private string[] _dialogueNodes;

    private bool _playerInRange;
    private int _currentDialogueIndex = 0;

    private InputAction _interactAction;
    private InputAction _continueAction;
    private DialogueRunner _dialogueRunner;

    // Finds the Yarn Spinner Dialogue Runner.
    private void Awake()
    {
        _dialogueRunner = FindFirstObjectByType<DialogueRunner>();

        if (_dialogueRunner == null)
        {
            Debug.LogError("Scene needs a Dialogue Runner.");
        }
    }

    // Checks for starting and continuing dialogue.
    private void Update()
    {
        if (_dialogueRunner == null)
        {
            return;
        }

        // Starts the character's current dialogue when E is pressed nearby.
        if (_playerInRange && _interactAction != null && _interactAction.WasPressedThisFrame() && !_dialogueRunner.IsDialogueRunning)
        {
            StartCurrentDialogue();
        }

        // Continues an active conversation with Space.
        if (_continueAction != null && _continueAction.WasPressedThisFrame() && _dialogueRunner.IsDialogueRunning)
        {
            _dialogueRunner.RequestNextLine();
        }
    }

    // Starts the currently selected Yarn dialogue node.
    private void StartCurrentDialogue()
    {
        if (_dialogueNodes == null || _dialogueNodes.Length == 0)
        {
            Debug.LogWarning($"{gameObject.name} has no dialogue nodes assigned.");
            return;
        }

        _dialogueRunner.StartDialogue(_dialogueNodes[_currentDialogueIndex]);

        // Moves to the next dialogue for the next interaction.
        if (_currentDialogueIndex < _dialogueNodes.Length - 1)
        {
            _currentDialogueIndex++;
        }
    }

    // Gets the player's input actions when they enter interaction range.
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            _playerInRange = true;

            PlayerInput playerInput = other.GetComponent<PlayerInput>();

            if (playerInput != null)
            {
                _interactAction = playerInput.actions["Interact"];
                _continueAction = playerInput.actions["ContinueDialogue"];
            }
        }
    }

    // Stops new interactions when the player leaves the interaction area.
    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            _playerInRange = false;
            _interactAction = null;
        }
    }
}