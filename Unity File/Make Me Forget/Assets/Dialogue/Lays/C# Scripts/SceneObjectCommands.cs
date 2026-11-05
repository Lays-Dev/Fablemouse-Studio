using UnityEngine;
using Yarn.Unity;

public class SceneObjectCommands : MonoBehaviour
{
    public DialogueRunner dialogueRunner;
    public GameObject[] objects;

    void Awake()
    {
        dialogueRunner.AddCommandHandler<string>("show", Show);
        dialogueRunner.AddCommandHandler<string>("hide", Hide);
    }

    void Show(string objectName)
    {
        Set(objectName, true);
    }

    void Hide(string objectName)
    {
        Set(objectName, false);
    }

    void Set(string objectName, bool on)
    {
        foreach (GameObject obj in objects)
        {
            if (obj != null && obj.name == objectName)
            {
                obj.SetActive(on);
                return;
            }
        }

        Debug.LogWarning("show/hide: no object named " + objectName + " in the list");
    }
}