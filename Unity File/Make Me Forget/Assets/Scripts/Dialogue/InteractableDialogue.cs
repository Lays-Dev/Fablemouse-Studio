using UnityEngine;
using UnityEngine.InputSystem;
using Yarn.Unity;
public class InteractableDialogue : MonoBehaviour
{
    private bool _playerInRange;
    private InputAction _interactAction;
    private DialogueRunner _dialogueRunner;

    private void Awake()
    {
        _dialogueRunner = FindFirstObjectByType<DialogueRunner>();
        if (_dialogueRunner == null)
        {
            Debug.LogError("Scene needs a Dialogue Runner.");
        }
    }
    private void Update()
    {
        if (_playerInRange && _interactAction != null && _dialogueRunner != null)
        {
            if (_interactAction.WasPressedThisFrame() && !_dialogueRunner.IsDialogueRunning)
            {
                _dialogueRunner.StartDialogue("Start");
            }
        }
    }
    // Checks when player enters interactable area
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            _playerInRange = true;
            PlayerInput playerInput = other.GetComponent<PlayerInput>();
            if (playerInput != null)
            {
                _interactAction = playerInput.actions["Interact"];
            }
        }
    }

    //Checks when player leaves interactable area
    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            _playerInRange = false;
            _interactAction = null;
        }
    }
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }   
}
