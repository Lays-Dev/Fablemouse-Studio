using UnityEngine;
using TMPro;

public class DialogueSpeaker : MonoBehaviour
{
    // Name Yarn uses to identify this character.
    [SerializeField] private string _speakerName;

    // This character's dialogue canvas.
    [SerializeField] private GameObject _dialogueBubble;

    // Text displayed inside this character's dialogue bubble.
    [SerializeField] private TMP_Text _dialogueText;

    public string SpeakerName => _speakerName;

    // Hides this character's dialogue when the scene starts.
    private void Start()
    {
        HideDialogue();
    }

    // Shows this character's dialogue bubble and updates its text.
    public void ShowDialogue(string dialogue)
    {
        if (_dialogueBubble == null)
        {
            Debug.LogError($"{gameObject.name} is missing its Dialogue Bubble.");
            return;
        }

        if (_dialogueText == null)
        {
            Debug.LogError($"{gameObject.name} is missing its Dialogue Text.");
            return;
        }

        _dialogueText.text = dialogue;
        _dialogueBubble.SetActive(true);
    }

    // Hides this character's dialogue bubble.
    public void HideDialogue()
    {
        if (_dialogueBubble != null)
        {
            _dialogueBubble.SetActive(false);
        }
    }
}
