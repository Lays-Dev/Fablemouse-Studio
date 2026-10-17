using UnityEngine;
using UnityEngine.EventSystems;

// The goal of the minigame is to click Big Green Bug
// When clicked, Big Green Bug will be in the inventory slot.
// The inventory slot will apper, the exit/return button will apper, the bug image will disapear. 

public class BigBug : MonoBehaviour, IPointerClickHandler
{
    public GameObject panel;
    public GameObject bigBugImage;
    public GameObject returnPointButton;

    private bool panelIsOpen = false;

    void Start()
    {
        // Hide the Inventory panel when the game starts.
        if (panel != null)
        {
            panel.SetActive(false);
        }
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        Debug.Log("Big bug clicked!");

        if (panel != null)
        {
            panelIsOpen = !panelIsOpen;
            panel.SetActive(panelIsOpen);
            returnPointButton.gameObject.SetActive(true);
            bigBugImage.gameObject.SetActive(false);
        }
        else
        {
            Debug.LogWarning("Panel has not been assigned!");
        }
    }
}
