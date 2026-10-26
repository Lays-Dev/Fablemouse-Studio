using UnityEngine;
using Yarn.Unity;

public class OpeningDialogue : MonoBehaviour
{
    [SerializeField] private DialogueRunner _dialogueRunner;
    [SerializeField] private string _startNode = "Counting";

    private void Start()
    {
        // Only play the introduction when starting a new game.
        if (GameSaveManager.Instance != null &&
            GameSaveManager.Instance.IsLoadingGame)
        {
            return;
        }

        if (_dialogueRunner != null)
        {
            _dialogueRunner.StartDialogue(_startNode);
        }
    }
}

