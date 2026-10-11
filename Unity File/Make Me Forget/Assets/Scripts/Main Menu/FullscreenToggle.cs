using UnityEngine;

public class FullscreenToggle : MonoBehaviour
{
    [SerializeField] private int windowedWidth = 1280;
    [SerializeField] private int windowedHeight = 720;

    public void EnableFullscreen()
    {
            //windowedWidth = Screen.width;
            //windowedHeight = Screen.height;
            Resolution nativeRes = Screen.currentResolution;
            Screen.SetResolution(nativeRes.width, nativeRes.height, FullScreenMode.FullScreenWindow);
    }

    public void DisableFullscreen()
    {
            Screen.SetResolution(windowedWidth, windowedHeight, FullScreenMode.Windowed);
    }

    public void ToggleFullscreen(bool isFullscreen)
    {
        switch (isFullscreen)
        {
            case true:
                EnableFullscreen();
                Debug.Log("Fullscreen mode enabled");
                break;
            case false:
                DisableFullscreen();
                Debug.Log("Windowed mode disabled");
                break;
        }
    }
}
