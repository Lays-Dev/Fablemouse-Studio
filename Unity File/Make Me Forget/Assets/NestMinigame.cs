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
   // [Tooltip("Assign the Continue Button here.")]
    //public Button ContinueButton;
    [Tooltip("Assign the Break Eggs Button here.")]
    public Button BreakEggsButton;
 
 
 
 
    [Tooltip("Assign the Close Instructions Button here.")]
    public Button StartButton;
  
  


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
 
 
 
   
   
    [Tooltip("Assign the Instructions Image here.")]
    public Image StartImage;
 
 
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
    //public string hungryBirds = "event:/SFX/Mechanics/Fish eggs/Hungry FB";
   
   




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
        //ContinueButton.onClick.AddListener(ContinueButtonClicked);
        BreakEggsButton.onClick.AddListener(BreakEggsButtonClicked);
        
        
        
        StartButton.onClick.AddListener(StartButtonClicked);
        
        
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
        //ContinueButton.gameObject.SetActive(false);
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
       // RuntimeManager.PlayOneShot(hungryBirds);
        //Debug.Log("Hungry Bird Fish sound played!");
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