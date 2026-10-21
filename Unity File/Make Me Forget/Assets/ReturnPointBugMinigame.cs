using UnityEngine;

public class ReturnPointBugMinigame : MonoBehaviour
{

    public GameObject bugRockCanvas;

    public void CloseCanvas()
    {
        bugRockCanvas.gameObject.SetActive(false);
    }
}
