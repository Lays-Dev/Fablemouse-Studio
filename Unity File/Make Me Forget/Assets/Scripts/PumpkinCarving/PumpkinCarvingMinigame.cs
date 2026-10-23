using UnityEngine;
using UnityEngine.Events;

public class PumpkinCarvingMinigame : MonoBehaviour
{
    public GameObject root;
    public Camera minigameCamera;
    public PumpkinCarver carver;
    public CarvingTemplate[] templates;

    [Header("Hooks for your game + UI")]
    public UnityEvent onStarted;
    public UnityEvent<float> onTimerTick;
    public UnityEvent<CarvingResult> onFinished;
    public UnityEvent onClosed;

    [Header("Win / Lose")]
    [Tooltip("Stars needed to win. 1 star = the template's One Star accuracy (50% by default).")]
    [Range(1, 3)] public int starsToWin = 1;
    public UnityEvent<CarvingResult> onWin;   // reward, advance the story, etc.
    public UnityEvent<CarvingResult> onLose;  // offer a retry, etc.

    public CarvingTemplate Current { get; private set; }
    public bool Running { get; private set; }
    public bool Won { get; private set; }
    float timeLeft;
    int currentIndex;

    void Awake()
    {
        if (root) root.SetActive(false);
    }

    public void StartMinigame(int templateIndex = 0)
    {
        currentIndex = Mathf.Clamp(templateIndex, 0, templates.Length - 1);
        Current = templates[currentIndex];
        root.SetActive(true);
        if (minigameCamera) { carver.cam = minigameCamera; minigameCamera.gameObject.SetActive(true); }
        carver.Begin(Current);
        timeLeft = Current.timeLimit;
        Won = false;
        Running = true;
        onStarted?.Invoke();
    }

    void Update()
    {
        if (!Running) return;
        timeLeft -= Time.unscaledDeltaTime;
        onTimerTick?.Invoke(Mathf.Max(0f, timeLeft));
        if (timeLeft <= 0f) Finish();
    }

    public void Finish()
    {
        if (!Running) return;
        Running = false;
        carver.InputEnabled = false;
        carver.SetLit(true);
        var result = CarvingScorer.Score(carver.Depth, carver.Target, carver.Allowed, Current);
        Won = result.stars >= starsToWin;   // set before events so listeners can read it
        onFinished?.Invoke(result);
        if (Won) onWin?.Invoke(result); else onLose?.Invoke(result);
    }

    /// Restart the same pumpkin design. Used by the Try Again button
    public void Retry() => StartMinigame(currentIndex);

    public CarvingResult Preview() =>
        CarvingScorer.Score(carver.Depth, carver.Target, carver.Allowed, Current);

    public void Close()
    {
        if (minigameCamera) minigameCamera.gameObject.SetActive(false);
        root.SetActive(false);
        onClosed?.Invoke();
    }
}