using System.Collections;
using UnityEngine;

// When loading a save, jumps the camera to the player instead of gliding over.
[RequireComponent(typeof(SideScrollerCamera))]
public class CameraSnapOnLoad : MonoBehaviour
{
    private IEnumerator Start()
    {
        GameSaveManager manager = GameSaveManager.Instance;
        if (manager == null || !manager.IsLoadingGame)
        {
            Debug.Log("CameraSnapOnLoad: not loading a save, no snap.");
            yield break;
        }

        // Wait until the save has put Lona back in her spot.
        yield return new WaitUntil(() => !manager.IsLoadingGame);

        // Give physics a step so her new position has fully applied.
        yield return new WaitForFixedUpdate();
        yield return null;

        SideScrollerCamera follow = GetComponent<SideScrollerCamera>();
        follow.SnapToTarget();
        Debug.Log("CameraSnapOnLoad: snapped to " + follow.target.position);
    }
}