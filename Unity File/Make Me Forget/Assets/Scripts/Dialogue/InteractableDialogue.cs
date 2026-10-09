using UnityEngine;
using UnityEngine.InputSystem;
using Yarn.Unity;

public class InteractableDialogue : MonoBehaviour
{
    // Yarn dialogue nodes this character can start.
    [SerializeField] private string[] _dialogueNodes;

    private bool _playerInRange;
    private int _currentDialogueIndex = 0;

    private InputAction _interactAction;
    private InputAction _continueAction;
    private DialogueRunner _dialogueRunner;

    // Finds the Dialogue Runner and the player's input actions.
    private void Awake()
    {
        _dialogueRunner = FindFirstObjectByType<DialogueRunner>();

        if (_dialogueRunner == null)
        {
            Debug.LogError("Scene needs a Dialogue Runner.");
        }

        PlayerInput playerInput = FindFirstObjectByType<PlayerInput>();

        if (playerInput == null)
        {
            Debug.LogError("Scene needs a PlayerInput component.");
            return;
        }

        _continueAction = playerInput.actions["ContinueDialogue"];
        _interactAction = playerInput.actions["Interact"];

        if (_continueAction == null)
        {
            Debug.LogError("PlayerInput needs a ContinueDialogue action.");
        }
    }

    // Checks for interaction and dialogue continuation.
    private void Update()
    {
        if (_dialogueRunner == null)
        {
            return;
        }

        // E starts dialogue when the player is nearby.
        if (_playerInRange &&
            _interactAction != null &&
            _interactAction.WasPressedThisFrame() &&
            !_dialogueRunner.IsDialogueRunning)
        {
            StartCurrentDialogue();
        }

        // Space advances dialogue regardless of interaction range.
        if (_continueAction != null &&
            _continueAction.WasPressedThisFrame() &&
            _dialogueRunner.IsDialogueRunning)
        {
            _dialogueRunner.RequestNextLine();
        }
    }

    // Starts the current dialogue node.
    private void StartCurrentDialogue()
    {
        if (_dialogueNodes == null || _dialogueNodes.Length == 0)
        {
            Debug.LogWarning($"{gameObject.name} has no dialogue nodes assigned.");
            return;
        }

        _dialogueRunner.StartDialogue(_dialogueNodes[_currentDialogueIndex]);

        // Advances to the next node for future interactions.
        if (_currentDialogueIndex < _dialogueNodes.Length - 1)
        {
            _currentDialogueIndex++;
        }
    }

    // Detects when the player enters interaction range.
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            _playerInRange = true;
        }
    }

    // Detects when the player leaves interaction range.
    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            _playerInRange = false;
        }
    }
}