using UnityEngine;
using Yarn.Unity;

public class ProximityDialogue : MonoBehaviour
{
    [Header("Dialogue")]
    // optional, if its empty it finds the dialogue runner by itself
    [SerializeField] private DialogueRunner _dialogueRunner;

    //[Header("Yarn Nodes")]
    // type the yarn node name here (like NestTree or Rock)
    //[SerializeField] private string _dialogueNode = "NestTree";
    [Header("Yarn Nodes")]
    [SerializeField] private string _dialogueNode = "Rock_Approach_Check";

    private bool _running;
    private bool _walkedAway;
    private bool _done;

    private void Start()
    {
        if (_dialogueRunner == null)
        {
            _dialogueRunner = FindFirstObjectByType<DialogueRunner>();
        }

        if (_dialogueRunner == null)
        {
            Debug.LogError("ProximityDialogue: No DialogueRunner found!");
            return;
        }

        _dialogueRunner.onDialogueComplete.AddListener(OnFinished);
    }

    // walk close and dialogue shows up 
    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player") || _done ||
            _dialogueRunner.IsDialogueRunning)
            return;

        // Don't interrupt another dialogue.
        if (_dialogueRunner == null || _dialogueRunner.IsDialogueRunning)
        {
            Debug.Log("Cannot start rock dialogue. Another dialogue may be running.");
            return;
        }

        _running = true;
        _walkedAway = false;

        Debug.Log("Starting Yarn node: " + _dialogueNode);

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

        private void OnDestroy()
    {
        if (_dialogueRunner != null)
        {
            _dialogueRunner.onDialogueComplete.RemoveListener(OnFinished);
        }
    }
}