using UnityEngine;
using Yarn.Unity;

public class ProximityDialogue : MonoBehaviour
{
    // optional, if its empty it finds the dialogue runner by itself
    [SerializeField] private DialogueRunner _dialogueRunner;

    // type the yarn node name here (like NestTree or Rock)
    [SerializeField] private string _dialogueNode = "NestTree";

    private bool _running;
    private bool _walkedAway;
    private bool _done;

    private void Start()
    {
        if (_dialogueRunner == null)
        {
            _dialogueRunner = FindFirstObjectByType<DialogueRunner>();
        }

        _dialogueRunner.onDialogueComplete.AddListener(OnFinished);
    }

    // walk close and dialogue shows up 
    private void OnTriggerEnter(Collider other)
    {
        Debug.Log($"{other.name} entered, tag: {other.tag}, dialogue running: {_dialogueRunner.IsDialogueRunning}");

        if (!other.CompareTag("Player") || _done || _dialogueRunner.IsDialogueRunning) return;

        _running = true;
        _walkedAway = false;
        _dialogueRunner.StartDialogue(_dialogueNode);
    }

    // walk away and dialogue goes away 
    private void OnTriggerExit(Collider other)
    {
        if (!other.CompareTag("Player") || !_running) return;

        _walkedAway = true;
        _dialogueRunner.Stop();
    }

    //  clicked through it and never shows again 
    private void OnFinished()
    {
        if (!_running) return;

        _running = false;

        if (!_walkedAway) _done = true;
    }
}