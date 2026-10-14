using UnityEngine;
using UnityEngine.InputSystem;
using Yarn.Unity;

public class DialogueContinueInput : MonoBehaviour
{
    private DialogueRunner _dialogueRunner;
    private InputAction _continueAction;

    // Finds the Dialogue Runner and player input.
    private void Awake()
    {
        _dialogueRunner = GetComponent<DialogueRunner>();

        PlayerInput playerInput = FindFirstObjectByType<PlayerInput>();

        if (playerInput != null)
        {
            _continueAction = playerInput.actions["ContinueDialogue"];
        }
    }

    // Advances dialogue when the continue button is pressed.
    private void Update()
    {
        if (_dialogueRunner == null || _continueAction == null)
        {
            return;
        }

        if (_continueAction.WasPressedThisFrame() &&
            _dialogueRunner.IsDialogueRunning)
        {
            _dialogueRunner.RequestNextLine();
        }
    }
}
