using UnityEngine;
using UnityEngine.UI;

public class TutorialMinigameScript : MonoBehaviour
{

#region Inspector 

    [Tooltip("Assign the Nest Button here.")]
    public Button NestButton;
    [Tooltip("Assign the UI Nest Image here.")]
    public Image NestImage;

#endregion
#region Start

    void Start()
    {
        NestButton.onClick.AddListener(NestButtonClicked);

    }

#endregion
#region Nest Button

    void NestButtonClicked()
    {
        Debug.Log("Nest Button was clicked.");
        NestImage.gameObject.SetActive(true);
    }

#endregion

}