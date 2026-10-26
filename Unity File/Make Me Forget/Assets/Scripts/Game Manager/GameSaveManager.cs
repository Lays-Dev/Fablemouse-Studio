using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameSaveManager : MonoBehaviour
{
    public static GameSaveManager Instance { get; private set; }
    public bool IsLoadingGame { get; private set; }
    private string _savePath;
    private GameSaveData _saveData;

    private void Awake()
    {
        // Only one save manager should exist.
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);

        _savePath = Path.Combine(
            Application.persistentDataPath, "save.json");
    }

    public void SaveGame()
    {
        _saveData = new GameSaveData();
        _saveData.sceneName = SceneManager.GetActiveScene().name;

        // Find every component that supports our save interface.
        MonoBehaviour[] components =
            FindObjectsByType<MonoBehaviour>(
                FindObjectsInactive.Include,
                FindObjectsSortMode.None);

        HashSet<string> usedIds = new HashSet<string>();

        foreach (MonoBehaviour component in components)
        {
            if (component is not ISaveable saveable)
                continue;

            if (string.IsNullOrWhiteSpace(saveable.SaveId))
            {
                Debug.LogError(
                    $"Missing SaveId on {component.name}", component);
                return;
            }

            if (!usedIds.Add(saveable.SaveId))
            {
                Debug.LogError(
                    $"Duplicate SaveId: {saveable.SaveId}", component);
                return;
            }

            SaveEntry entry = new SaveEntry();
            entry.id = saveable.SaveId;
            entry.json = saveable.CaptureState();

            _saveData.objects.Add(entry);
        }

        string json = JsonUtility.ToJson(_saveData, true);
        File.WriteAllText(_savePath, json);

        Debug.Log("Game saved: " + _savePath);
    }

    public bool HasSave()
    {
        return File.Exists(_savePath);
    }

    public void LoadGame()
    {
        if (!HasSave())
        {
            Debug.LogWarning("No saved game found.");
            return;
        }

        string json = File.ReadAllText(_savePath);
        _saveData = JsonUtility.FromJson<GameSaveData>(json);

        if (_saveData == null ||
            string.IsNullOrEmpty(_saveData.sceneName))
        {
            Debug.LogError("Save file is invalid.");
            return;
        }

        // Load the saved scene before restoring its objects.
        SceneManager.sceneLoaded += OnSceneLoaded;
        IsLoadingGame = true;
        SceneManager.LoadScene(_saveData.sceneName);
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;

        // Wait one frame so scene objects can finish Start().
        StartCoroutine(RestoreNextFrame());
    }

    private System.Collections.IEnumerator RestoreNextFrame()
    {
        yield return null;

        MonoBehaviour[] components =
            FindObjectsByType<MonoBehaviour>(
                FindObjectsInactive.Include,
                FindObjectsSortMode.None);

        Dictionary<string, ISaveable> saveables =
            new Dictionary<string, ISaveable>();

        foreach (MonoBehaviour component in components)
        {
            if (component is ISaveable saveable)
            {
                if (string.IsNullOrWhiteSpace(saveable.SaveId) ||
                    saveables.ContainsKey(saveable.SaveId))
                {
                    Debug.LogError(
                        $"Invalid or duplicate SaveId on {component.name}",
                        component);
                    yield break;
                }

                saveables.Add(saveable.SaveId, saveable);
            }
        }

        foreach (SaveEntry entry in _saveData.objects)
        {
            if (saveables.TryGetValue(entry.id, out ISaveable saveable))
                saveable.RestoreState(entry.json);
            else
                Debug.LogWarning("Saveable not found: " + entry.id);
        }
        IsLoadingGame = false; // Loading is finished.

        Debug.Log("Game loaded successfully.");
    }
}

