using UnityEngine;

public class DialogueBubbleManager : MonoBehaviour
{
    private DialogueSpeaker _currentSpeaker;

    // Shows dialogue for the current speaker and hides the previous speaker.
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

    // Hides whichever character is currently speaking.
    public void HideCurrentDialogue()
    {
        if (_currentSpeaker != null)
        {
            _currentSpeaker.HideDialogue();
            _currentSpeaker = null;
        }
    }
}
