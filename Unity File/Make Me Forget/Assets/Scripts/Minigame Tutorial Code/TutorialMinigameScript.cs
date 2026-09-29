using UnityEngine;
using UnityEngine.UI;

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

// Animations

    [Tooltip("Assign the Broken Eggs Image here.")]
    public Animator BrokenEggsAnimation;



#endregion

#region Variables

private bool EggsBroken = false;


#endregion
#region Start

    void Start()
    {
        NestButton.onClick.AddListener(NestButtonClicked);
        ReturnButton.onClick.AddListener(ReturnButtonClicked);
        ContinueButton.onClick.AddListener(ContinueButtonClicked);
        BreakEggsButton.onClick.AddListener(BreakEggsButtonClicked);

    }

#endregion

    void Update()
    {
        AnimatorStateInfo state = BrokenEggsAnimation.GetCurrentAnimatorStateInfo(0);
        if (state.normalizedTime >= 1f && !BrokenEggsAnimation.IsInTransition(0))
        {
            BirbImage.gameObject.SetActive(true);
            ReturnButton.gameObject.SetActive(true);

        }

    }

#region Nest Button

    void NestButtonClicked()
    {
        Debug.Log("Nest Button was clicked.");
        NestImage.gameObject.SetActive(true);
        ReturnButton.gameObject.SetActive(true);
    }

#endregion

    void ReturnButtonClicked()
    {
        Debug.Log("Return Button was clicked.");

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
        NestImage.gameObject.SetActive(false);
        ReturnButton.gameObject.SetActive(false);
        ContinueButton.gameObject.SetActive(false);
        InstructionsImage.gameObject.SetActive(false);
        NestButton.gameObject.SetActive(true);
    }

    void BreakEggsButtonClicked()
    {
        ReturnButton.gameObject.SetActive(false);
        EggsImage.gameObject.SetActive(false);
        BrokenEggsImage.gameObject.SetActive(true);
        BreakEggsButton.gameObject.SetActive(false);
        EggsBroken = true;
        Debug.Log("EggsBroken variable = " + EggsBroken);
    }

}