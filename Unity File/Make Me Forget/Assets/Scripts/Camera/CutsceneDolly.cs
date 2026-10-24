using System.Collections;
using UnityEngine;


// starts moving then stays locked at the end <3
public class CutsceneDolly : MonoBehaviour
{
    [Header("References")]
    [SerializeField] Transform cam;         
    [SerializeField] Transform startPoint;   
    [SerializeField] Transform endPoint;     // only used if Camera Controller is empty
    [SerializeField] CutsceneHandoff handoff;
    [SerializeField] SideScrollerCamera cameraController; // the end spot is taken from this

    [Header("Timing")]
    [SerializeField] float startDelay = 0f;  
    [SerializeField] float duration = 6f;    
    [SerializeField] AnimationCurve ease = AnimationCurve.EaseInOut(0, 0, 1, 1);

    Vector3 endPos;
    Quaternion endRot;

    // Start automatically when the scene plays
    IEnumerator Start()
    {
        // Gameplay rotation = the camera's rotation before we move it
        endRot = endPoint != null && cameraController == null ? endPoint.rotation : cam.rotation;
        endPos = endPoint != null ? endPoint.position : cam.position;
        UpdateEndSpot();

        // Snapping
        cam.SetPositionAndRotation(startPoint.position, startPoint.rotation);

        if (startDelay > 0f)
            yield return new WaitForSeconds(startDelay);

        // Moving
        for (float t = 0f; t < duration; t += Time.deltaTime)
        {
            UpdateEndSpot(); // follow wherever she is right now

            float k = ease.Evaluate(t / duration);
            cam.SetPositionAndRotation(
                Vector3.Lerp(startPoint.position, endPos, k),
                Quaternion.Slerp(startPoint.rotation, endRot, k));
            yield return null;
        }

        // Lockeeed in!
        UpdateEndSpot();
        cam.SetPositionAndRotation(endPos, endRot);

        // TEMPORARY: remove once the wake-up animation triggers the handoff
        if (handoff != null)
            handoff.EndCutscene();
    }

    // Asks the camera script where gameplay would put the camera right now
    void UpdateEndSpot()
    {
        if (cameraController == null) return;

        cam.rotation = endRot;           
        cameraController.SnapToTarget(); 
        endPos = cam.position;           
    }
}