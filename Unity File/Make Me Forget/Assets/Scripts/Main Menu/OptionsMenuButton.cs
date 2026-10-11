using UnityEngine;

public class OptionsMenuButton : MonoBehaviour
{
    public GameObject optionsMenu;
    public void OnClick()
    {
        optionsMenu.SetActive(!optionsMenu.activeSelf);
    }
}
