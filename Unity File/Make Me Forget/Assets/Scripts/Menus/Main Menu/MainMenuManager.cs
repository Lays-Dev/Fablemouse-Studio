using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuManager : MonoBehaviour
{
    // Singleton design pattern.
    public static MainMenuManager _;

    [SerializeField] private bool _debugMode;
    [SerializeField] private string _sceneToLoadAfterClickingPlay;
    [SerializeField] GameObject _MainMenuContainer;
    [SerializeField] GameObject _CreditsMenuContainer;
    [SerializeField] GameObject _SettingsMenuContainer;
    public enum MainMenuButtons 
    {
        Play,
        Options,
        Credits,
        Quit,
        Continue
    };

    public enum OptionsButtons 
    {
        Return
    };

    public enum CreditsButtons 
    {
        Return
    };

    public void Awake()
    {
        if (_ == null)
        {
            _ = this;
        }
        else
        {
            Debug.LogError("There are more than 1 MainMenuManager's in the scene.");
        }
    }

    private void Start()
    {
        OpenMenu(_MainMenuContainer);
    }

    public void MainMenuButtonClicked(MainMenuButtons buttonClicked)
    {
        DebugMessage("Button Clicked: " + buttonClicked.ToString());

        switch (buttonClicked)
        {
            case MainMenuButtons.Continue:
                ContinueGame();
                break;
            case MainMenuButtons.Play:
                PlayClicked();
                break;
            case MainMenuButtons.Options:
                OpenOptionsMenu();
                break;
            case MainMenuButtons.Credits:
                OpenCreditsMenu();
                break;
            case MainMenuButtons.Quit:
                QuitGame();
                break;
            default:
                Debug.Log("Button clicked that wasn't implemented in MainMenuManager script switch case.");
                break;

        }
    }

    private void DebugMessage(string message)
    {
        if (_debugMode)
        {
            Debug.Log(message);
        }
    }

    public void PlayClicked()
    {
        SceneManager.LoadScene(_sceneToLoadAfterClickingPlay);
    }
    public void ContinueGame()
    {
        // Use the persistent save manager to load the saved game.
        if (GameSaveManager.Instance != null)
        {
            GameSaveManager.Instance.LoadGame();
        }
        else
        {
            Debug.LogError("GameSaveManager not found!");
        }
    }

    public void OpenCreditsMenu()
    {
        OpenMenu(_CreditsMenuContainer);
    }

    public void OpenOptionsMenu()
    {
        OpenMenu(_SettingsMenuContainer);
    }

    public void ReturnToMainMenu()
    {
        OpenMenu(_MainMenuContainer);
    }

    public void QuitGame()
    {
        #if UNITY_EDITOR
            UnityEditor.EditorApplication.ExitPlaymode();
        #else
            Application.Quit();
        #endif
    }

    public void CreditsButtonClicked(CreditsButtons buttonClicked)
    {
        switch(buttonClicked)
        {
            case CreditsButtons.Return:
                ReturnToMainMenu();
                break;
        }
    }

    public void SettingsButtonClicked(OptionsButtons buttonClicked)
    {
        switch(buttonClicked)
        {
            case OptionsButtons.Return:
                ReturnToMainMenu();
                break;
        }
    }

    public void OpenMenu(GameObject menuToOpen)
    {
        _MainMenuContainer.SetActive(menuToOpen == _MainMenuContainer);
        _CreditsMenuContainer.SetActive(menuToOpen == _CreditsMenuContainer);
        _SettingsMenuContainer.SetActive(menuToOpen == _SettingsMenuContainer);
    }


}
