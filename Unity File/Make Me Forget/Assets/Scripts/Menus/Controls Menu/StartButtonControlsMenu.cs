using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class StartButtonControlsMenu : MonoBehaviour
{
    // The name of the scene you want to change to goes here. 
    // It must be spelled exactly the same.
    // The Controls menu will go to the Tutorial level.
    [SerializeField] private string tutoralSceneName = "Tutorial";
    [SerializeField] private string mainMenuSceneName = "Main Menu";


    // Naming the method something obvious.
    public void StartTutorialLevel()
    {
        SceneManager.LoadScene(tutoralSceneName);
    }

    public void ReturnToMainMenu()
    {
        SceneManager.LoadScene(mainMenuSceneName);
    }

}
