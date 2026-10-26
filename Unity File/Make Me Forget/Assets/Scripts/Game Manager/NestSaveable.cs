using System;
using UnityEngine;

public class NestSaveable : MonoBehaviour, ISaveable
{
    [SerializeField] private string _saveId = "nest_eggs";
    [SerializeField] private NestMinigame _nestMinigame;

    public string SaveId => _saveId;

    [Serializable]
    private class NestState
    {
        public int eggClickCount;
    }

    public string CaptureState()
    {
        // Read the current egg-cracking progress.
        NestState state = new NestState();
        state.eggClickCount = _nestMinigame.GetEggClickCount();

        // Convert the progress into saveable JSON.
        return JsonUtility.ToJson(state);
    }

    public void RestoreState(string json)
    {
        // Read the saved egg-cracking progress.
        NestState state = JsonUtility.FromJson<NestState>(json);

        // Restore the eggs and their appearance.
        _nestMinigame.RestoreEggClickCount(state.eggClickCount);
    }
}
