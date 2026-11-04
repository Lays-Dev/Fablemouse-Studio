// This script connects Yarn Spinner Dialogue to Little Lona's Animations. 
// Starting with, her counting animation at game start.

using UnityEngine;
// Required library Yarn, grants acess to the [YarnCommand]
using Yarn.Unity;

public class LittleLonaDialogueAnamation : MonoBehaviour
{
    // Singleton reference with Instance Variable ( single instance )
    // Yarn commands can find this animation controler without needing to create another reference
    public static LittleLonaDialogueAnamation Instance;


    [Header("Little Lona")]
    [Tooltip("Put Little Lona GameObject here.")]
    public Animator lonaAnimator;

    [Header("Animation")]
    [Tooltip("Type the exact name of the IsCounting bool here. It is in the Anamator window.")]
    public string countingParameter = "IsCounting";

    [Header("Player Controls")]
    [Tooltip("Drag the script responsible for player movement, jumping, and interaction here.")]
    public Behaviour[] playerControlScripts;
    public PlayerMovement playerMovement;

    private void Awake()
    {
        // Singleton. this is This Script
        Instance = this;
    }
    
    // Disables or enables all assigned player control scripts.
    private void SetPlayerControls(bool enabled)
    {
        foreach (Behaviour controlScript in playerControlScripts)
        {
            if (controlScript != null)
            {
                controlScript.enabled = enabled;
            }
        }
    }

// The Yarn Script runs these commands!

    [YarnCommand("start_counting")]
    public static void StartCounting()
    {
        if (Instance == null) return;

        // Start the counting animation.
        Instance.lonaAnimator.SetBool(
            Instance.countingParameter, true);

        // Prevent player movement.
        Instance.playerMovement.FreezePlayer();

//        Debug.Log("START COUNTING!");
    }

    [YarnCommand("stop_counting")]
    public static void StopCounting()
    {
        if (Instance == null) return;

        // Stop the counting animation.
        Instance.lonaAnimator.SetBool(
            Instance.countingParameter, false);

        // Allow player movement again.
        Instance.playerMovement.UnfreezePlayer();

   //     Debug.Log("STOP COUNTING!");
    }
}
