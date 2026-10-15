using UnityEngine;

public class CutsceneHandoff : MonoBehaviour
{
    [SerializeField] PlayerMovement playerMovement;
    [SerializeField] SideScrollerCamera cameraController;

    bool ended;

    // Turn off movement + camera follow as soon as the scene starts
    void Awake()
    {
        playerMovement.enabled = false;
        cameraController.enabled = false;
    }

    public void EndCutscene()
    {
        if (ended) return; // only hand off once for meow
        ended = true;

        playerMovement.enabled = true;
        cameraController.enabled = true;
    }
}