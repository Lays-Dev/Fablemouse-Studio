using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuMusicManager : MonoBehaviour
{
    private static MainMenuMusicManager instance;

    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }
        instance = this;

        DontDestroyOnLoad(gameObject);
    }

    private void OnEnable()
    {
        // Listen for scene changes.
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDisable()
    {
        // Stop listening when disabled or destroyed.
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        // Stop menu music when entering the Tutorial.
        if (scene.name == "Tutorial")
        {
            Destroy(gameObject);
        }
    }
}
