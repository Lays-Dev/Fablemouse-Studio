using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Yarn.Unity;

// Plays the opening dialogue on a new game only, and saves Yarn's variables.
[RequireComponent(typeof(DialogueRunner))]
public class DialogueSaveable : MonoBehaviour, ISaveable
{
    [SerializeField] private string saveId = "Yarn_Dialogue";
    [SerializeField] private string openingNode = "Little_Lona";

    private DialogueRunner runner;
    public string SaveId => saveId;

    // JsonUtility can't save dictionaries, so the values go in lists.
    [Serializable] private class Entry<T> { public string key; public T value; }

    [Serializable]
    private class State
    {
        public List<Entry<float>> floats;
        public List<Entry<string>> strings;
        public List<Entry<bool>> bools;
    }

    private void Awake() => runner = GetComponent<DialogueRunner>();

    private void Start()
    {
        bool loading = GameSaveManager.Instance != null && GameSaveManager.Instance.IsLoadingGame;
        if (!loading) runner.StartDialogue(openingNode);
    }

    public string CaptureState()
    {
        var (floats, strings, bools) = runner.VariableStorage.GetAllVariables();
        return JsonUtility.ToJson(new State
        {
            floats = ToList(floats),
            strings = ToList(strings),
            bools = ToList(bools)
        });
    }

    public void RestoreState(string json)
    {
        State s = JsonUtility.FromJson<State>(json);
        runner.VariableStorage.SetAllVariables(ToDict(s.floats), ToDict(s.strings), ToDict(s.bools), true);
    }

    private static List<Entry<T>> ToList<T>(Dictionary<string, T> d) =>
        d.Select(p => new Entry<T> { key = p.Key, value = p.Value }).ToList();

    private static Dictionary<string, T> ToDict<T>(List<Entry<T>> l) =>
        l.ToDictionary(e => e.key, e => e.value);
}