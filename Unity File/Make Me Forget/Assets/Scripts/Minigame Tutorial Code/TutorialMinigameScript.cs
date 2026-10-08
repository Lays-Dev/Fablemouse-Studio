using UnityEngine;
using UnityEngine.UI;
using FMODUnity;

public class TutorialMinigameScript : MonoBehaviour
{

#region Inspector 

// Buttons

    [Tooltip("Assign the Nest Button here.")]
    public Button NestButton;
    [Tooltip("Assign the Return Button here.")]
    public Button ReturnButton;
    [Tooltip("Assign the Continue Button here.")]
    public Button ContinueButton;
    [Tooltip("Assign the Break Eggs Button here.")]
    public Button BreakEggsButton;
    [Tooltip("Assign the Background 2 Button here.")]
    public Button Background2Button;
    [Tooltip("Assign the Background 1 Button here.")]
    public Button Background1Button;
    [Tooltip("Assign the Bug Button here.")]
    public Button BugButton;
    [Tooltip("Assign the Close Instructions Button here.")]
    public Button StartButton;
    [Tooltip("Assign the Branch Button here.")]
    public Button BranchButton; 
    [Tooltip("Assign the Win Button here.")]
    public Button WinButton; 
    [Tooltip("Assign the End Minigame Button here.")]
    public Button EndButton;

// Images

    [Tooltip("Assign the UI Nest Image here.")]
    public Image NestImage;
    [Tooltip("Assign the Instructions Image here.")]
    public Image InstructionsImage;
    [Tooltip("Assign the Broken Eggs Image here.")]
    public Image BrokenEggsImage;
    [Tooltip("Assign the Eggs Image here.")]
    public Image EggsImage;
    [Tooltip("Assign the Birb Image here.")]
    public Image BirbImage;
    [Tooltip("Assign the Background 2 Image here.")]
    public Image Background2Image;
    [Tooltip("Assign the Bug Image here.")]
    public Image BugImage;
    [Tooltip("Assign the Smudge Image here.")]
    public Image SmudgeImage;
    [Tooltip("Assign the Instructions Image here.")]
    public Image StartImage;
    [Tooltip("Assign the Tree Image here.")]
    public Image TreeImage;
    [Tooltip("Assign the Win Image here.")]
    public Image WinImage;

    public GameObject canvas;
    public static bool foundJason = false;


// Animations

    [Tooltip("Assign the Broken Eggs Image here.")]
    public Animator BrokenEggsAnimation;

// SFX

    public string interactArrow = "event:/UI/Front end/select/Young/UI Select";
    public string eggsCracking = "event:/SFX/Mechanics/Fish eggs/Hatching";
    public string bugSplat = "event:/SFX/Mechanics/Bug/Splat";




#endregion

#region Variables

private bool EggsBroken = false;
private bool nest = false;


#endregion
#region Start

    void Start()
    {
        NestButton.onClick.AddListener(NestButtonClicked);
        ReturnButton.onClick.AddListener(ReturnButtonClicked);
        ContinueButton.onClick.AddListener(ContinueButtonClicked);
        BreakEggsButton.onClick.AddListener(BreakEggsButtonClicked);
        Background2Button.onClick.AddListener(Background2ButtonClicked);
        Background1Button.onClick.AddListener(Background1ButtonClicked);
        BugButton.onClick.AddListener(BugButtonClicked);
        StartButton.onClick.AddListener(StartButtonClicked);
        BranchButton.onClick.AddListener(BranchButtonClicked);
        WinButton.onClick.AddListener(WinButtonClicked);
        EndButton.onClick.AddListener(EndButtonClicked);
    }

#endregion

    void Update()
    {
        if (nest == true)
        {
            AnimatorStateInfo state = BrokenEggsAnimation.GetCurrentAnimatorStateInfo(0);
            if (state.normalizedTime >= 1f && !BrokenEggsAnimation.IsInTransition(0))
            {
                BirbImage.gameObject.SetActive(true);
                ReturnButton.gameObject.SetActive(true);

            }
        }

    }

    void StartButtonClicked()
    {
        Debug.Log("Start Button was clicked.");

        RuntimeManager.PlayOneShot(interactArrow);
        Debug.Log("Arrow Interaction sound played!");

        StartImage.gameObject.SetActive(false);
        StartButton.gameObject.SetActive(false);
        Background2Button.gameObject.SetActive(true);
    }

#region Nest Button

    void NestButtonClicked()
    {

        Debug.Log("Nest Button was clicked.");

        RuntimeManager.PlayOneShot(interactArrow);
        Debug.Log("Arrow Interaction sound played!");

        NestImage.gameObject.SetActive(true);
        ReturnButton.gameObject.SetActive(true);
    }

#endregion

    void ReturnButtonClicked()
    {
        Debug.Log("Return Button was clicked.");
        RuntimeManager.PlayOneShot(interactArrow);
        Debug.Log("Arrow Interaction sound played!");

        if (EggsBroken == true)
        {
            NestImage.gameObject.SetActive(false);
            ReturnButton.gameObject.SetActive(false);
            NestButton.gameObject.SetActive(false);
        }
        else
        {
            NestImage.gameObject.SetActive(false);
            ReturnButton.gameObject.SetActive(false);
            NestButton.gameObject.SetActive(true);
        }

    }

    void ContinueButtonClicked()
    {
        Debug.Log("Continue Button was clicked.");
        RuntimeManager.PlayOneShot(interactArrow);
        Debug.Log("Arrow Interaction sound played!");
        NestImage.gameObject.SetActive(false);
        ReturnButton.gameObject.SetActive(false);
        ContinueButton.gameObject.SetActive(false);
        InstructionsImage.gameObject.SetActive(false);
        NestButton.gameObject.SetActive(true);
    }

    void BreakEggsButtonClicked()
    {
        RuntimeManager.PlayOneShot(interactArrow);
        Debug.Log("Arrow Interaction sound played!");
        RuntimeManager.PlayOneShot(eggsCracking);
        Debug.Log("Eggs Cracking sound played!");
        nest = true;
        ReturnButton.gameObject.SetActive(false);
        EggsImage.gameObject.SetActive(false);
        BrokenEggsImage.gameObject.SetActive(true);
        BreakEggsButton.gameObject.SetActive(false);
        EggsBroken = true;
        Debug.Log("EggsBroken variable = " + EggsBroken);
    }

    void Background2ButtonClicked()
    {
        Debug.Log("Background 2 Button was clicked.");

        RuntimeManager.PlayOneShot(interactArrow);
        Debug.Log("Arrow Interaction sound played!");

        Background2Image.gameObject.SetActive(true);
        Background2Button.gameObject.SetActive(false);

        if (EggsBroken == true)
        {
            Background1Button.gameObject.SetActive(false);
        }

    }

        void Background1ButtonClicked()
    {
        Debug.Log("Background 1 Button was clicked.");

        RuntimeManager.PlayOneShot(interactArrow);
        Debug.Log("Arrow Interaction sound played!");

        Background2Image.gameObject.SetActive(false);
        Background2Button.gameObject.SetActive(true);


    }

    void BugButtonClicked()
    {
        Debug.Log("Bug Button was clicked.");
        RuntimeManager.PlayOneShot(interactArrow);
        Debug.Log("Arrow Interaction sound played!");
        RuntimeManager.PlayOneShot(bugSplat);
        Debug.Log("Bug Splat sound played!");
        BugImage.gameObject.SetActive(false);
        SmudgeImage.gameObject.SetActive(true);
        BugButton.gameObject.SetActive(false);


    }

    void BranchButtonClicked()
    {
        Debug.Log("Branch Button was clicked.");

        RuntimeManager.PlayOneShot(interactArrow);
        Debug.Log("Arrow Interaction sound played!");

        TreeImage.gameObject.SetActive(true);
        BranchButton.gameObject.SetActive(false);


    }

        void WinButtonClicked()
    {
        Debug.Log("Win Button was clicked.");

        RuntimeManager.PlayOneShot(interactArrow);
        Debug.Log("Arrow Interaction sound played!");

        WinImage.gameObject.SetActive(true);
        TreeImage.gameObject.SetActive(false);
        Background1Button.gameObject.SetActive(false);
        BugButton.gameObject.SetActive(false);
        Background2Image.gameObject.SetActive(false);



    }

    void EndButtonClicked()
    {
        Debug.Log("End Button was clicked.");

        RuntimeManager.PlayOneShot(interactArrow);
        Debug.Log("Arrow Interaction sound played!");
        foundJason = true;

        canvas.SetActive(false);

    }

}