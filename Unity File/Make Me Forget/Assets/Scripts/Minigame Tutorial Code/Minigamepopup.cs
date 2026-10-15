using UnityEngine;
using UnityEngine.Events;

public class MiniGamePopup : MonoBehaviour
{
    [Header("Player Control")]
    public Behaviour[] disableWhileOpen;
    public bool freeCursor = true;
    public bool allowCancel = true;
    public KeyCode cancelKey = KeyCode.Escape;

    [Header("Events")]
    public UnityEvent onOpened;
    public UnityEvent onCompleted;
    public UnityEvent onClosed;

    public bool IsOpen { get; private set; }
    public int CurrentPart { get; private set; }

    CursorLockMode prevLockMode;
    bool prevCursorVisible;

    void Awake()
    {
        gameObject.SetActive(false);
    }

    void Update()
    {
        if (IsOpen && allowCancel && Input.GetKeyDown(cancelKey))
            Close();
    }

    // open sesame 

    public void Open(int partIndex)
    {
        CurrentPart = partIndex;
        IsOpen = true;
        gameObject.SetActive(true);

        foreach (var b in disableWhileOpen)
            if (b) b.enabled = false;

        if (freeCursor)
        {
            prevLockMode = Cursor.lockState;
            prevCursorVisible = Cursor.visible;
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }

        onOpened.Invoke();
    }

    // they won!! call this from the minigame 

    public void Complete()
    {
        if (!IsOpen) return;

        if (MiniGameProgress.Instance != null)
            MiniGameProgress.Instance.MarkDone(CurrentPart);

        onCompleted.Invoke();
        Close();
    }

    // bye bye popup

    public void Close()
    {
        if (!IsOpen) return;
        IsOpen = false;

        foreach (var b in disableWhileOpen)
            if (b) b.enabled = true;

        if (freeCursor)
        {
            Cursor.lockState = prevLockMode;
            Cursor.visible = prevCursorVisible;
        }

        onClosed.Invoke();
        gameObject.SetActive(false);
    }
}