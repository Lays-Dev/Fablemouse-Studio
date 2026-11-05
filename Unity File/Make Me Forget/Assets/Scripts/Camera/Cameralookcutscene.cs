using System.Collections;
using UnityEngine;
using Yarn.Unity;

public class CameraLookCutscene : MonoBehaviour
{
    [Header("Camera")]
    public Transform cam;
    public SideScrollerCamera cameraController;
    public Transform lookSpot;

    [Header("Player")]
    public PlayerMovement playerMovement;

    [Header("Wait For")]
    public MiniGamePopup minigamePopup;
    public DialogueRunner dialogueRunner;

    [Header("Dialogue")]
    public string nodeName;

    [Header("Timing")]
    public float moveTime = 1.5f;
    public float holdTime = 1.5f;

    private bool hasPlayed = false;

    void OnEnable()
    {
        if (hasPlayed) return;
        hasPlayed = true;

        StartCoroutine(PlayCutscene());
    }

    IEnumerator PlayCutscene()
    {
        if (minigamePopup != null)
            yield return new WaitUntil(() => !minigamePopup.IsOpen);

        if (dialogueRunner != null)
            yield return new WaitUntil(() => !dialogueRunner.IsDialogueRunning);

        playerMovement.FreezePlayer();
        cameraController.enabled = false;

        Vector3 homePos = cam.position;
        Quaternion homeRot = cam.rotation;

        yield return Glide(homePos, homeRot, lookSpot.position, lookSpot.rotation);

        if (dialogueRunner != null && !string.IsNullOrEmpty(nodeName))
        {
            dialogueRunner.StartDialogue(nodeName);
            yield return new WaitUntil(() => !dialogueRunner.IsDialogueRunning);
        }
        else
        {
            yield return new WaitForSeconds(holdTime);
        }

        yield return Glide(lookSpot.position, lookSpot.rotation, homePos, homeRot);

        cameraController.enabled = true;
        cameraController.SnapToTarget();
        playerMovement.UnfreezePlayer();
    }

    IEnumerator Glide(Vector3 fromPos, Quaternion fromRot, Vector3 toPos, Quaternion toRot)
    {
        for (float t = 0f; t < moveTime; t += Time.deltaTime)
        {
            float k = Mathf.SmoothStep(0f, 1f, t / moveTime);
            cam.SetPositionAndRotation(
                Vector3.Lerp(fromPos, toPos, k),
                Quaternion.Slerp(fromRot, toRot, k));
            yield return null;
        }

        cam.SetPositionAndRotation(toPos, toRot);
    }
}