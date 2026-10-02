using System.Collections;
using UnityEngine;


// starts moving then stays locked at the end <3
public class CutsceneDolly : MonoBehaviour
{
    [Header("References")]
    [SerializeField] Transform cam;         
    [SerializeField] Transform startPoint;   
    [SerializeField] Transform endPoint;     
    [SerializeField] CutsceneHandoff handoff;

    [Header("Timing")]
    [SerializeField] float startDelay = 0f;  
    [SerializeField] float duration = 6f;    
    [SerializeField] AnimationCurve ease = AnimationCurve.EaseInOut(0, 0, 1, 1);

    // Start automatically when the scene plays
    IEnumerator Start()
    {
        // Snapping
        cam.SetPositionAndRotation(startPoint.position, startPoint.rotation);

        if (startDelay > 0f)
            yield return new WaitForSeconds(startDelay);

        // Moving
        for (float t = 0f; t < duration; t += Time.deltaTime)
        {
            float k = ease.Evaluate(t / duration);
            cam.SetPositionAndRotation(
                Vector3.Lerp(startPoint.position, endPoint.position, k),
                Quaternion.Slerp(startPoint.rotation, endPoint.rotation, k));
            yield return null;
        }

        // Lockeeed in!
        cam.SetPositionAndRotation(endPoint.position, endPoint.rotation);

        // TEMPORARY: remove once the wake-up animation triggers the handoff
        if (handoff != null)
            handoff.EndCutscene();
    }
}