using UnityEngine;
using Yarn.Unity;

public class ProximityDialogue : MonoBehaviour
{
    // type the yarn node name here (like Baby_Birds or Rock etc)
    [SerializeField] private string _node;

    private DialogueRunner _dialogueRunner;
    private bool _running;
    private bool _walkedAway;
    private bool _done;

    private void Start()
    {
        // finds the dialogue runner by itself so u dont have to drag it in
        _dialogueRunner = FindFirstObjectByType<DialogueRunner>();
        _dialogueRunner.onDialogueComplete.AddListener(OnFinished);
    }

    // walk close and dialogue shows up
    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player") || _done || _dialogueRunner.IsDialogueRunning) return;

        _running = true;
        _walkedAway = false;
        _dialogueRunner.StartDialogue(_node);
    }

    // walk away and dialogue goes away 
    private void OnTriggerExit(Collider other)
    {
        if (!other.CompareTag("Player") || !_running) return;

        _walkedAway = true;
        _dialogueRunner.Stop();
    }

    // for when u click this time it goes away
    private void OnFinished()
    {
        if (!_running) return;

        _running = false;

        if (!_walkedAway) _done = true;
    }
}