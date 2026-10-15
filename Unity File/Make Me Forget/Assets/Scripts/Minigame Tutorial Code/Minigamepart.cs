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

    bool playerInRange;

    void Start()
    {
        if (interactPrompt) interactPrompt.SetActive(false);
    }

    void Update()
    {
        if (!useBuiltInTrigger || !playerInRange) return;

        if (Input.GetKeyDown(interactKey))
            Interact();
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

    // ---- trigger zone stuff ----

    void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag(playerTag)) return;
        playerInRange = true;

        bool done = MiniGameProgress.Instance != null && MiniGameProgress.Instance.IsDone(partIndex);
        if (interactPrompt && !done) interactPrompt.SetActive(true);
    }

    void OnTriggerExit(Collider other)
    {
        if (!other.CompareTag(playerTag)) return;
        playerInRange = false;
        if (interactPrompt) interactPrompt.SetActive(false);
    }
}