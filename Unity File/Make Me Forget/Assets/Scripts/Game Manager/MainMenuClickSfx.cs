
using UnityEngine;
using UnityEngine.UI;
using FMODUnity;

public class MainMenuClickSfx : MonoBehaviour
{
    [SerializeField] private EventReference clickSound;

    private void Awake()
    {
        // Find all buttons, including inactive ones.
        Button[] buttons = GetComponentsInChildren<Button>(true);

        // Add the click sound to every button.
        foreach (Button button in buttons)
        {
            button.onClick.AddListener(PlayClickSound);
        }
    }

    private void PlayClickSound()
    {
        // Play the selected FMOD event.
        if (!clickSound.IsNull)
        {
            RuntimeManager.PlayOneShot(clickSound);
        }
    }
}

