using UnityEngine;
using Yarn.Unity;

public class ProximityDialogue : MonoBehaviour
{
    [SerializeField] private DialogueRunner _dialogueRunner;
    [SerializeField] private string _dialogueNode = "NestTree";

    private bool _hasTriggered;

    private void OnTriggerEnter(Collider other)
    {
        // Only activate when the player enters the area
        if (!other.CompareTag("Player") || _hasTriggered)
            return;

        // Don't interrupt another conversation
        if (_dialogueRunner.IsDialogueRunning)
            return;

        _hasTriggered = true;
        _dialogueRunner.StartDialogue(_dialogueNode);
    }
}
