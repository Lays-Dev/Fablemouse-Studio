using UnityEngine;

public class DialogueBubbleManager : MonoBehaviour
{
    private DialogueSpeaker _currentSpeaker;

    // Changes which character is currently displaying dialogue.
    public void ShowDialogue(DialogueSpeaker speaker, string dialogue)
    {
        if (speaker == null)
        {
            return;
        }

        if (_currentSpeaker != null && _currentSpeaker != speaker)
        {
            _currentSpeaker.HideDialogue();
        }

        speaker.ShowDialogue(dialogue);

        _currentSpeaker = speaker;
    }

    // Hides the currently visible dialogue bubble.
    public void HideCurrentDialogue()
    {
        if (_currentSpeaker != null)
        {
            _currentSpeaker.HideDialogue();
            _currentSpeaker = null;
        }
    }
}
