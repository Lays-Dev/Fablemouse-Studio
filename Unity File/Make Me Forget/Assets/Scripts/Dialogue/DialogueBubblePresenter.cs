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
    // Receives each line from Yarn and sends it to the correct character.
    public override YarnTask RunLineAsync(LocalizedLine line,LineCancellationToken token)
    {
        if (_bubbleManager == null)
        {
            return YarnTask.CompletedTask;
        }

        string speakerName = line.CharacterName;
        string dialogue = line.TextWithoutCharacterName.Text;

        DialogueSpeaker[] speakers = FindObjectsByType<DialogueSpeaker>(FindObjectsSortMode.None);

        foreach (DialogueSpeaker speaker in speakers)
        {
            if (speaker.SpeakerName == speakerName)
            {
                _bubbleManager.ShowDialogue(speaker, dialogue);
                break;
            }
        }

        return YarnTask.CompletedTask;
    }

    // Hides the final speech bubble when the conversation ends.
    public override YarnTask OnDialogueCompleteAsync()
    {
        if (_bubbleManager != null)
        {
            _bubbleManager.HideCurrentDialogue();
        }

        return YarnTask.CompletedTask;
    }
}

