using UnityEngine;
using Yarn.Unity;

// When this object turns on, start a Yarn node.
public class StartDialogueOnEnable : MonoBehaviour
{
    public DialogueRunner dialogueRunner;
    public string nodeName;

    void OnEnable()
    {
        Debug.Log(name + " turned on, starting " + nodeName);

        if (dialogueRunner.IsDialogueRunning)
        {
            Debug.Log("Skipped, another dialogue is still running!");
            return;
        }

        dialogueRunner.StartDialogue(nodeName);
    }
}