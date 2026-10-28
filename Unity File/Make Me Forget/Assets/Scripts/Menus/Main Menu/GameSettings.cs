
using UnityEngine;
using UnityEngine.UI;
using FMODUnity;
using FMOD.Studio;

public class GameSettings : MonoBehaviour
{
    [SerializeField] private Toggle fullscreenToggle;
    [SerializeField] private Slider masterVolumeSlider;
    [SerializeField] private Slider musicVolumeSlider;
    [SerializeField] private Slider sfxVolumeSlider;

    [SerializeField] private string masterBusPath = "bus:/";
    [SerializeField] private string musicBusPath = "bus:/Music";
    [SerializeField] private string sfxBusPath = "bus:/SFX";

    private Bus masterBus;
    private Bus musicBus;
    private Bus sfxBus;

    private void Start()
    {
        // Restore fullscreen even if the toggle is not assigned.
        int defaultValue = Screen.fullScreen ? 1 : 0;
        bool savedFullscreen = PlayerPrefs.GetInt("Fullscreen", defaultValue) == 1;

        Screen.fullScreen = savedFullscreen;

        if (fullscreenToggle != null)
        {
            fullscreenToggle.SetIsOnWithoutNotify(savedFullscreen);
        }

        // Load saved audio settings.
        float masterVolume = PlayerPrefs.GetFloat("MasterVolume", 1f);
        float musicVolume = PlayerPrefs.GetFloat("MusicVolume", 1f);
        float sfxVolume = PlayerPrefs.GetFloat("SFXVolume", 1f);

        // Find buses only when their paths exist.
        masterBus = GetOptionalBus(masterBusPath);
        musicBus = GetOptionalBus(musicBusPath);
        sfxBus = GetOptionalBus(sfxBusPath);

        // Apply volumes only to valid buses.
        if (masterBus.isValid())
            masterBus.setVolume(masterVolume);

        if (musicBus.isValid())
            musicBus.setVolume(musicVolume);

        if (sfxBus.isValid())
            sfxBus.setVolume(sfxVolume);

        // Update only the sliders that are assigned.
        if (masterVolumeSlider != null)
            masterVolumeSlider.SetValueWithoutNotify(masterVolume);

        if (musicVolumeSlider != null)
            musicVolumeSlider.SetValueWithoutNotify(musicVolume);

        if (sfxVolumeSlider != null)
            sfxVolumeSlider.SetValueWithoutNotify(sfxVolume);
    }

    private Bus GetOptionalBus(string path)
    {
        // Skip empty paths.
        if (string.IsNullOrWhiteSpace(path))
            return default(Bus);

        // Try to find the bus without crashing on missing paths.
        try
        {
            return RuntimeManager.GetBus(path);
        }
        catch (BusNotFoundException)
        {
            Debug.LogWarning("FMOD bus not found: " + path);
            return default(Bus);
        }
    }

    public void SetFullscreen(bool isFullscreen)
    {
        Screen.fullScreen = isFullscreen;

        PlayerPrefs.SetInt("Fullscreen", isFullscreen ? 1 : 0);
        PlayerPrefs.Save();
    }

    public void SetMasterVolume(float volume)
    {
        if (masterBus.isValid())
            masterBus.setVolume(volume);

        PlayerPrefs.SetFloat("MasterVolume", volume);
        PlayerPrefs.Save();
    }

    public void SetMusicVolume(float volume)
    {
        if (musicBus.isValid())
            musicBus.setVolume(volume);

        PlayerPrefs.SetFloat("MusicVolume", volume);
        PlayerPrefs.Save();
    }

    public void SetSFXVolume(float volume)
    {
        if (sfxBus.isValid())
            sfxBus.setVolume(volume);

        PlayerPrefs.SetFloat("SFXVolume", volume);
        PlayerPrefs.Save();
    }
}

