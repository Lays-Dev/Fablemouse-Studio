using UnityEngine;
using Yarn.Unity;

public class DialogueBubblePresenter : DialoguePresenterBase
{
    private DialogueBubbleManager _bubbleManager;

    // Finds the dialogue bubble manager when the scene starts.
    private void Awake()
    {
        _bubbleManager = FindFirstObjectByType<DialogueBubbleManager>();

        if (_bubbleManager == null)
        {
            Debug.LogError("Scene needs a DialogueBubbleManager.");
        }
    }

    // Called by Yarn when a conversation begins.
    public override YarnTask OnDialogueStartedAsync()
    {
        return YarnTask.CompletedTask;
    }

    // Shows one Yarn line and waits for the player to continue.
    public override async YarnTask RunLineAsync(
        LocalizedLine line,
        LineCancellationToken token)
    {
        if (_bubbleManager == null)
        {
            return;
        }

        string speakerName = line.CharacterName;
        string dialogue = line.TextWithoutCharacterName.Text;

        DialogueSpeaker[] speakers =
            FindObjectsByType<DialogueSpeaker>(FindObjectsSortMode.None);

        DialogueSpeaker matchingSpeaker = null;

        // Finds the character whose speaker name matches the Yarn line.
        foreach (DialogueSpeaker speaker in speakers)
        {
            if (speaker.SpeakerName == speakerName)
            {
                matchingSpeaker = speaker;
                break;
            }
        }

        if (matchingSpeaker == null)
        {
            Debug.LogWarning(
                $"No DialogueSpeaker found with the name '{speakerName}'."
            );

            return;
        }

        // Shows this line above the correct character.
        _bubbleManager.ShowDialogue(matchingSpeaker, dialogue);

        // Keeps this line on screen until RequestNextLine() is called.
        await YarnTask.WaitUntilCanceled(token.NextContentToken)
            .SuppressCancellationThrow();
    }

    // Hides the final bubble when the conversation finishes.
    public override YarnTask OnDialogueCompleteAsync()
    {
        if (_bubbleManager != null)
        {
            _bubbleManager.HideCurrentDialogue();
        }

        return YarnTask.CompletedTask;
    }
}

