using UnityEngine;

public class ApplicationQuit : MonoBehaviour
{
    public void OnClick()
    {
        Debug.Log("Closing application...");
        Application.Quit();
    }
}
