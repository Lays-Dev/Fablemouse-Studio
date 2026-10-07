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

    public CarvingTemplate Current { get; private set; }
    public bool Running { get; private set; }
    float timeLeft;

    void Awake()
    {
        if (root) root.SetActive(false);
    }

    public void StartMinigame(int templateIndex = 0)
    {
        Current = templates[Mathf.Clamp(templateIndex, 0, templates.Length - 1)];
        root.SetActive(true);
        if (minigameCamera) { carver.cam = minigameCamera; minigameCamera.gameObject.SetActive(true); }
        carver.Begin(Current);
        timeLeft = Current.timeLimit;
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
        onFinished?.Invoke(result);
    }

    public CarvingResult Preview() =>
        CarvingScorer.Score(carver.Depth, carver.Target, carver.Allowed, Current);

    public void Close()
    {
        if (minigameCamera) minigameCamera.gameObject.SetActive(false);
        root.SetActive(false);
        onClosed?.Invoke();
    }
}