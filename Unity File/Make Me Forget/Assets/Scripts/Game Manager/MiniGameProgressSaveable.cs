using System;
using UnityEngine;

public class MiniGameProgressSaveable : MonoBehaviour, ISaveable
{
    [SerializeField] private string _saveId = "minigame_progress";
    [SerializeField] private MiniGameProgress _progress;

    public string SaveId => _saveId;

    [Serializable]
    private class ProgressState
    {
        public bool[] completedParts;
    }

    public string CaptureState()
    {
        ProgressState state = new ProgressState();

        // Collect the current completion states.
        state.completedParts = _progress.GetCompletedParts();

        return JsonUtility.ToJson(state);
    }

    public void RestoreState(string json)
    {
        ProgressState state =
            JsonUtility.FromJson<ProgressState>(json);

        // Restore the completed minigames.
        _progress.LoadCompletedParts(state.completedParts);
    }
}

