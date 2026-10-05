using UnityEngine;
using TMPro;

public class DialogueSpeaker : MonoBehaviour
{
    [SerializeField] private string _speakerName;
    [SerializeField] private GameObject _dialogueBubble;
    [SerializeField] private TMP_Text _dialogueText;

    public string SpeakerName => _speakerName;

    public void ShowDialogue(string dialogue)
    {
        if (_dialogueBubble == null || _dialogueText == null)
        {
            return;
        }
        _dialogueText.text = dialogue;
        _dialogueBubble.SetActive(true);
    }
    public void HideDialogue()
    {
        if (_dialogueBubble != null)
        {
            _dialogueBubble.SetActive(false);   
        }
    }
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    private void Start()
    {
        HideDialogue();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
