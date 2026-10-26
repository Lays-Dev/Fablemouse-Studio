using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem;

// put this on the pause canvas, NOT on the panel
public class PauseMenu : MonoBehaviour
{
    public GameObject pausePanel;
    public string mainMenuScene = "MainMenu";

    // drag in stuff that uses E (PlayerMovement, BranchPickup, TreeSequence, MiniGameParts)
    public Behaviour[] scriptsToDisable;

    private bool isPaused = false;

    private void Start()
    {
        pausePanel.SetActive(false);
    }

    private void Update()
    {
        // escape = pause / unpause
        if (Keyboard.current != null && Keyboard.current.escapeKey.wasPressedThisFrame)
        {
            if (isPaused) Resume();
            else Pause();
        }
    }

    // freeze!!
    public void Pause()
    {
        isPaused = true;
        pausePanel.SetActive(true);
        Time.timeScale = 0f;

        // no sneaky E presses while paused
        foreach (var script in scriptsToDisable)
            if (script != null) script.enabled = false;
    }

    // ok back to the game
    public void Resume()
    {
        isPaused = false;
        pausePanel.SetActive(false);
        Time.timeScale = 1f;

        foreach (var script in scriptsToDisable)
            if (script != null) script.enabled = true;
    }
    // Save the current game progress.
    public void SaveGame()
    {
        if (GameSaveManager.Instance != null)
        {
            GameSaveManager.Instance.SaveGame();
            Debug.Log("Game saved successfully!");
        }
        else
        {
            Debug.LogWarning("GameSaveManager not found!");
        }
    }
    // bye back to the menu (unfreeze first or the menu's frozen too lol)
    public void GoToMainMenu()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(mainMenuScene);
    }

    // rage quit (only works in the real build, not the editor)
    public void QuitGame()
    {
        Application.Quit();
    }
}