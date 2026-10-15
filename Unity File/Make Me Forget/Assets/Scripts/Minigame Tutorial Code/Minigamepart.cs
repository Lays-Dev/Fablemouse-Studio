using UnityEngine;

public class MiniGamePart : MonoBehaviour
{
    [Header("Setup")]
    public int partIndex;
    public MiniGamePopup popup;

    [Header("Trigger")]
    public bool useBuiltInTrigger = true;
    public KeyCode interactKey = KeyCode.E;
    public string playerTag = "Player";
    public GameObject interactPrompt;
    public bool debugLogs = true;

    bool playerInRange;

    void Start()
    {
        if (interactPrompt) interactPrompt.SetActive(false);
    }

    void Update()
    {
        if (!useBuiltInTrigger || !playerInRange) return;

        if (Input.GetKeyDown(interactKey))
        {
            if (debugLogs) Debug.Log($"{name}: E pressed, opening part {partIndex}");
            Interact();
        }
    }

    // ---- open the popup (or hook this into ur own interaction system) ----

    public void Interact()
    {
        if (popup == null)
        {
            Debug.LogWarning($"{name}: no popup assigned", this);
            return;
        }

        if (MiniGameProgress.Instance != null && MiniGameProgress.Instance.IsDone(partIndex))
            return;

        if (popup.IsOpen) return;

        if (interactPrompt) interactPrompt.SetActive(false);
        popup.Open(partIndex);
    }

    // ---- is this lona? (checks the collider, its rigidbody, and parents) ----

    bool IsPlayer(GameObject go, Component rb)
    {
        if (go.CompareTag(playerTag)) return true;
        if (rb && rb.CompareTag(playerTag)) return true;

        Transform t = go.transform.parent;
        while (t != null)
        {
            if (t.CompareTag(playerTag)) return true;
            t = t.parent;
        }
        return false;
    }

    void Enter(GameObject other, Component rb)
    {
        if (debugLogs) Debug.Log($"{name}: something entered -> {other.name} (tag: {other.tag})");
        if (!IsPlayer(other, rb)) return;

        playerInRange = true;
        if (debugLogs) Debug.Log($"{name}: player in range, press {interactKey}");

        bool done = MiniGameProgress.Instance != null && MiniGameProgress.Instance.IsDone(partIndex);
        if (interactPrompt && !done) interactPrompt.SetActive(true);
    }

    void Exit(GameObject other, Component rb)
    {
        if (!IsPlayer(other, rb)) return;
        playerInRange = false;
        if (interactPrompt) interactPrompt.SetActive(false);
    }

    // ---- 3D triggers ----

    void OnTriggerEnter(Collider other) => Enter(other.gameObject, other.attachedRigidbody);
    void OnTriggerExit(Collider other) => Exit(other.gameObject, other.attachedRigidbody);

    // ---- 2D triggers (in case lona uses 2D physics) ----

    void OnTriggerEnter2D(Collider2D other) => Enter(other.gameObject, other.attachedRigidbody);
    void OnTriggerExit2D(Collider2D other) => Exit(other.gameObject, other.attachedRigidbody);
}