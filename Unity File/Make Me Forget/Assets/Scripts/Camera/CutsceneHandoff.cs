using UnityEngine;

public class CutsceneHandoff : MonoBehaviour
{
    [SerializeField] PlayerMovement playerMovement;
    [SerializeField] SideScrollerCamera cameraController;

    bool ended;

    public void EndCutscene()
    {
        if (ended) return; // only hand off once for meow
        ended = true;

        playerMovement.enabled = true;
        cameraController.enabled = true;
    }
}