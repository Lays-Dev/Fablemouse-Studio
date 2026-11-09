using UnityEngine;
using UnityEngine.UI;
using FMODUnity;
using FMOD.Studio;
using System.Collections;
using Yarn.Unity;

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
    public GameObject BirdsImage;
    public Image DialougeImage;

    [Header("Yarn Dialogue")]
    [SerializeField] private DialogueRunner dialogueRunner;

    public string interactArrow =
        "event:/UI/Front end/select/Young/UI Select";

    public string eggsCrackingClick1 =
        "event:/SFX/Mechanics/Fish eggs/Hatching/Click 1";

    public string eggsCrackingClick2 =
        "event:/SFX/Mechanics/Fish eggs/Hatching/Click 2";

    [Tooltip("The minigame moves on after this many seconds, even if a sound hasn't finished.")]
    public float maxSoundWait = 5f;

    #endregion

    private int eggClickCount = 0;
    private bool isPlayingSound = false;

    public int GetEggClickCount()
    {
        // Return how many times the eggs have been cracked.
        return eggClickCount;
    }

    public void RestoreEggClickCount(int savedCount)
    {
        // Restore how many times the eggs were cracked.
        eggClickCount = Mathf.Clamp(savedCount, 0, 2);

        if (eggClickCount == 1)
        {
            EggsImage.sprite = EggsCrackedImage.sprite;
        }
        else if (eggClickCount == 2)
        {
            EggsImage.sprite = EggsBrokenImage.sprite;
            BirdsImage.gameObject.SetActive(true);
            BreakEggsButton.gameObject.SetActive(false);
            DialougeImage.gameObject.SetActive(true);
            ReturnPointButton.gameObject.SetActive(true);
        }
    }
    void Start()
    {
        BreakEggsButton.onClick.AddListener(BreakEggsButtonClicked);
        ReturnPointButton.onClick.AddListener(ReturnPointButtonClicked);

        // The return button starts active before eggs are broken.
        ReturnPointButton.gameObject.SetActive(true);

        if (dialogueRunner == null)
        {
            dialogueRunner = FindFirstObjectByType<DialogueRunner>();
        }
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
        yield return PlaySoundAndWait(interactArrow);

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

        // Play the egg cracking sound for this click.
        yield return PlaySoundAndWait(
            eggClickCount == 1 ? eggsCrackingClick1 : eggsCrackingClick2);

        Debug.Log("Egg sound finished playing!");

        // After the second click, show the birds.
        if (eggClickCount == 2)
        {
            BirdsImage.gameObject.SetActive(true);
            BreakEggsButton.gameObject.SetActive(false);
            DialougeImage.gameObject.SetActive(true);

            ReturnPointButton.gameObject.SetActive(true);
            ReturnPointButton.interactable = true;

            // Tell Yarn that the eggs have hatched.
            if (dialogueRunner != null)
            {
                dialogueRunner.StartDialogue("Egg_Click_2");
            }
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

    // Plays a sound and waits for it. A missing or stuck sound can't block the minigame.
    IEnumerator PlaySoundAndWait(string eventPath)
    {
        EventInstance sound;

        try
        {
            sound = RuntimeManager.CreateInstance(eventPath);
        }
        catch (EventNotFoundException)
        {
            Debug.LogError("FMOD event not found, skipping sound: " + eventPath);
            yield break;
        }

        sound.start();

        float giveUpTime = Time.unscaledTime + maxSoundWait;
        PLAYBACK_STATE state;

        do
        {
            yield return null;
            sound.getPlaybackState(out state);
        }
        while (state != PLAYBACK_STATE.STOPPED && Time.unscaledTime < giveUpTime);

        if (state != PLAYBACK_STATE.STOPPED)
        {
            sound.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
        }

        sound.release();
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
        // Close the popup properly if it is assigned.
        MiniGamePopup popup = FindFirstObjectByType<MiniGamePopup>();

        if (popup != null && popup.IsOpen)
        {
            popup.Close();
        }
        else
        {
            // Fallback if this minigame isn't managed by a popup.
            MinigameCanvas.SetActive(false);
        }
    }

    public bool AreEggsBroken()
    {
        return eggClickCount >= 2;
    }

}