using System.Collections;
using UnityEngine;
using Yarn.Unity;

// Each time PlayNext() is called, plays the next node in the list.
public class DialogueOnClick : MonoBehaviour
{
    public DialogueRunner dialogueRunner;
    public string[] nodeNames;

    private int count = 0;

    public void PlayNext()
    {
        if (count >= nodeNames.Length) return;

        string node = nodeNames[count];
        count++;

        StartCoroutine(SwitchTo(node));
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