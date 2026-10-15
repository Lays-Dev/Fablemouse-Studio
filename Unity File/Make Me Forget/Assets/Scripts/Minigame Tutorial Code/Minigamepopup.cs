using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class MiniGamePopup : MonoBehaviour
{
    [Header("Player Control")]
    public GameObject player;
    public string[] scriptsToDisable = { "PlayerMovement" };
    public bool freeCursor = true;
    public bool allowCancel = true;
    public KeyCode cancelKey = KeyCode.Escape;

    [Header("Events")]
    public UnityEvent onOpened;
    public UnityEvent onCompleted;
    public UnityEvent onClosed;

    public bool IsOpen { get; private set; }
    public int CurrentPart { get; private set; }

    readonly List<Behaviour> disabled = new List<Behaviour>();
    CursorLockMode prevLockMode;
    bool prevCursorVisible;

    void Awake()
    {
        // hide on start but don't fight Open() lol
        if (!IsOpen) gameObject.SetActive(false);
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

        FreezePlayer();

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

        UnfreezePlayer();

        if (freeCursor)
        {
            Cursor.lockState = prevLockMode;
            Cursor.visible = prevCursorVisible;
        }

        onClosed.Invoke();
        gameObject.SetActive(false);
    }

    // stop lona from wandering off

    void FreezePlayer()
    {
        disabled.Clear();
        if (player == null) return;

        foreach (string scriptName in scriptsToDisable)
        {
            var script = player.GetComponent(scriptName) as Behaviour;
            if (script == null)
            {
                Debug.LogWarning($"couldn't find '{scriptName}' on {player.name}, check the spelling bestie", this);
                continue;
            }
            if (script.enabled)
            {
                script.enabled = false;
                disabled.Add(script);
            }
        }
    }

    void UnfreezePlayer()
    {
        foreach (var script in disabled)
            if (script) script.enabled = true;
        disabled.Clear();
    }
}