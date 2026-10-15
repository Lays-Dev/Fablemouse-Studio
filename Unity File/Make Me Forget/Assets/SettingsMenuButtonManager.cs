using UnityEngine;

public class SettingsMenuButtonManager : MonoBehaviour
{
    [SerializeField] MainMenuManager.OptionsButtons _buttonType;

    public void ButtonClicked()
    {
        MainMenuManager._.SettingsButtonClicked(_buttonType);
    }
}
