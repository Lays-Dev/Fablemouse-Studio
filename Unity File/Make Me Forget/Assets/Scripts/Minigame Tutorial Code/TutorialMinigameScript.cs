using UnityEngine;
using UnityEngine.UI;

public class TutorialMinigameScript : MonoBehaviour
{

#region Inspector 

    [Tooltip("Assign the Nest Button here.")]
    public Button NestButton;
    [Tooltip("Assign the Return Button here.")]
    public Button ReturnButton;
    [Tooltip("Assign the UI Nest Image here.")]
    public Image NestImage;

#endregion
#region Start

    void Start()
    {
        NestButton.onClick.AddListener(NestButtonClicked);
        ReturnButton.onClick.AddListener(ReturnButtonClicked);

    }

#endregion
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
        NestImage.gameObject.SetActive(false);
        ReturnButton.gameObject.SetActive(false);
        NestButton.gameObject.SetActive(false);
    }

}