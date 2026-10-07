using UnityEngine;
using UnityEngine.InputSystem;
using Yarn.Unity;

public class InteractableDialogue : MonoBehaviour
{
    // Stores the Yarn dialogue nodes this character can start.
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
        PlayerInput playerInput = FindFirstObjectByType<PlayerInput>();
        _continueAction = playerInput.actions["ContinueDialogue"];
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

        // Starts this character's current dialogue when E is pressed nearby.
        if (_playerInRange &&
            _interactAction != null &&
            _interactAction.WasPressedThisFrame() &&
            !_dialogueRunner.IsDialogueRunning)
        {
            StartCurrentDialogue();
        }

        // Continues an active conversation with Space.
        if (_continueAction != null &&
            _continueAction.WasPressedThisFrame() &&
            _dialogueRunner.IsDialogueRunning)
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

        string nodeName = _dialogueNodes[_currentDialogueIndex];

        if (string.IsNullOrEmpty(nodeName))
        {
            Debug.LogWarning($"{gameObject.name} has an empty dialogue node.");
            return;
        }

        Debug.Log($"{gameObject.name} starting Yarn node: {nodeName}");

        _dialogueRunner.StartDialogue(nodeName);

        // Moves to the next dialogue after this interaction.
        // Once the final dialogue is reached, it continues using the final one.
        if (_currentDialogueIndex < _dialogueNodes.Length - 1)
        {
            _currentDialogueIndex++;
        }
    }

    // Gets the player's input actions when they enter the interaction area.
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            _playerInRange = true;

            PlayerInput playerInput = other.GetComponent<PlayerInput>();

            if (playerInput == null)
            {
                Debug.LogError("Player needs a PlayerInput component.");
                return;
            }

            _interactAction = playerInput.actions["Interact"];
            

            Debug.Log($"Player entered {gameObject.name}'s dialogue trigger.");
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