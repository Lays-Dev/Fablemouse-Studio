using UnityEngine;
using UnityEngine.InputSystem;

public class ForceQuit : MonoBehaviour
{
    // This handles the quit logic when triggered via PlayerInput component
    public void OnQuit(InputAction.CallbackContext context)
    {
        // Only trigger when the button is fully pressed down
        if (context.started)
        {
            Quit();
        }
    }

    public void Quit()
    {
        // Quit the game, build
        Application.Quit();

        // Quit Play Mode in the Unity Editor
        #if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
        #endif
    }
}
