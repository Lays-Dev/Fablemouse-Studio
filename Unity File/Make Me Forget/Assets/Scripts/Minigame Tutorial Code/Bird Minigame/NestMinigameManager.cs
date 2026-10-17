
using UnityEngine;
using UnityEngine.UI;
using FMODUnity;
using FMOD.Studio;
using System.Collections;

public class NestMinigame : MonoBehaviour
{
    #region Inspector

    public Button BreakEggsButton;
    [Tooltip("Assign the Return Point Button here.")]
public Button ReturnPointButton;

[Tooltip("Assign the entire minigame Canvas here.")]
public GameObject MinigameCanvas;

    public Image EggsImage;
    public Image EggsCrackedImage;
    public Image EggsBrokenImage;
    public Image BirdsImage;
    public Image DialougeImage;

    public string interactArrow =
        "event:/UI/Front end/select/Young/UI Select";

    public string eggsCracking =
        "event:/SFX/Mechanics/Fish eggs/Hatching";

    #endregion

    private int eggClickCount = 0;
    private bool isPlayingSound = false;

void Start()
{
    BreakEggsButton.onClick.AddListener(BreakEggsButtonClicked);
    ReturnPointButton.onClick.AddListener(ReturnPointButtonClicked);

    // The return button starts active before eggs are broken.
    ReturnPointButton.gameObject.SetActive(true);
}

    void BreakEggsButtonClicked()
    {
        // Prevent another click while the sounds are playing.
        if (isPlayingSound)
            return;

        StartCoroutine(PlayEggSounds());
    }

    IEnumerator PlayEggSounds()
    {
      isPlayingSound = true;
BreakEggsButton.interactable = false;
ReturnPointButton.interactable = false;
ReturnPointButton.gameObject.SetActive(false);

        // Play the interaction sound and wait for it to finish.
        EventInstance arrowSound =
            RuntimeManager.CreateInstance(interactArrow);

        arrowSound.start();

        yield return new WaitUntil(() =>
        {
            PLAYBACK_STATE state;
            arrowSound.getPlaybackState(out state);
            return state == PLAYBACK_STATE.STOPPED;
        });

        arrowSound.release();

        eggClickCount++;

        // First click: show cracked eggs.
        if (eggClickCount == 1)
        {
            EggsImage.sprite = EggsCrackedImage.sprite;
        }
        // Second click: show broken eggs.
        else if (eggClickCount == 2)
        {
            EggsImage.sprite = EggsBrokenImage.sprite;
        }

        // Play the egg cracking sound.
        EventInstance crackingSound =
            RuntimeManager.CreateInstance(eggsCracking);

        crackingSound.start();

        yield return new WaitUntil(() =>
        {
            PLAYBACK_STATE state;
            crackingSound.getPlaybackState(out state);
            return state == PLAYBACK_STATE.STOPPED;
        });

        crackingSound.release();

        Debug.Log("Egg sound finished playing!");

        // After the second click, show the birds.
        if (eggClickCount == 2)
        {
            BirdsImage.gameObject.SetActive(true);
            BreakEggsButton.gameObject.SetActive(false);
            DialougeImage.gameObject.SetActive(true);

            // Allow returning once the birds are active.
            ReturnPointButton.gameObject.SetActive(true);
            ReturnPointButton.interactable = true;
        }
        else
        {
            // Allow the second egg click.
            BreakEggsButton.interactable = true;

            // Keep the return button disabled between egg clicks.
            ReturnPointButton.gameObject.SetActive(false);
        }

        isPlayingSound = false;
    }

    void OnDestroy()
    {
        if (BreakEggsButton != null)
        {
            BreakEggsButton.onClick.RemoveListener(
                BreakEggsButtonClicked);
        }
    }

    void ReturnPointButtonClicked()
{
    // Disable the entire minigame Canvas.
    MinigameCanvas.SetActive(false);
}
}
