using System.Collections;
using UnityEngine;
using Yarn.Unity;

public class LoopingReminder : MonoBehaviour
{
    public DialogueRunner dialogueRunner;
    public string[] nodeNames;

    public GameObject onlyIfThisIsOn;
    public GameObject notIfThisIsOn;

    private int index = 0;

    public void Play()
    {
        if (onlyIfThisIsOn != null && !onlyIfThisIsOn.activeSelf) return;
        if (notIfThisIsOn != null && notIfThisIsOn.activeSelf) return;

        string node = nodeNames[index];
        index = (index + 1) % nodeNames.Length;

        dialogueRunner.StartCoroutine(SwitchTo(node));
    }

    IEnumerator SwitchTo(string node)
    {
        if (dialogueRunner.IsDialogueRunning)
        {
            dialogueRunner.Stop();
            yield return new WaitUntil(() => !dialogueRunner.IsDialogueRunning);
        }

        dialogueRunner.StartDialogue(node);
    }
}