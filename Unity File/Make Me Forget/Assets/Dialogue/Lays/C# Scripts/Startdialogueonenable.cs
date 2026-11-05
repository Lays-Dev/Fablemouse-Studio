using System.Collections;
using UnityEngine;
using Yarn.Unity;

// When this object turns on, start a Yarn node (only the first time)
public class StartDialogueOnEnable : MonoBehaviour
{
    public DialogueRunner dialogueRunner;
    public string nodeName;

    private bool hasPlayed = false;

    void OnEnable()
    {
        Debug.Log(name + " turned on (already played: " + hasPlayed + ")");

        if (hasPlayed) return;
        hasPlayed = true;

        dialogueRunner.StartCoroutine(SwitchTo(nodeName));
    }

    IEnumerator SwitchTo(string node)
    {
        // If something is already showing, close it first.
        if (dialogueRunner.IsDialogueRunning)
        {
            dialogueRunner.Stop();
            yield return new WaitUntil(() => !dialogueRunner.IsDialogueRunning);
        }

        dialogueRunner.StartDialogue(node);
    }
}