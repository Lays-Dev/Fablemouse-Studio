using UnityEngine;
using UnityEngine.Events;

public class MiniGameProgress : MonoBehaviour
{
    public static MiniGameProgress Instance { get; private set; }

    public int partCount = 3;
    public UnityEvent onAllComplete;
    public UnityEvent<int> onPartComplete;

    bool[] done;

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Debug.LogWarning("there's 2 MiniGameProgress in the scene bestie", this);
            Destroy(this);
            return;
        }
        Instance = this;
        done = new bool[partCount];
    }

    // checkers 

    public bool IsDone(int part) => part >= 0 && part < done.Length && done[part];

    public int CompletedCount
    {
        get
        {
            int n = 0;
            foreach (bool d in done) if (d) n++;
            return n;
        }
    }

    public bool AllDone => CompletedCount == partCount;

    // mark it done and check if we wonnn

    public void MarkDone(int part)
    {
        if (part < 0 || part >= done.Length || done[part]) return;

        done[part] = true;
        Debug.Log($"part {part} done ({CompletedCount}/{partCount})");
        onPartComplete.Invoke(part);

        if (AllDone)
        {
            Debug.Log("all parts done, we ate");
            onAllComplete.Invoke();
        }
    }
}