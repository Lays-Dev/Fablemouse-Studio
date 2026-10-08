using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;
#if ENABLE_INPUT_SYSTEM
using UnityEngine.InputSystem;
#endif

// Interaction point that loads the hospital door to next scene

public class SceneDoor : MonoBehaviour
{
    [Tooltip("Exact name of the scene to load. It must be in the Build Profiles / Build Settings scene list.")]
    public string sceneToLoad;
    public string playerTag = "Player";

    [Header("How it triggers")]
    [Tooltip("Off = player must press the key. On = loads as soon as the player walks in.")]
    public bool loadOnEnter = false;
#if ENABLE_INPUT_SYSTEM
    public Key interactKey = Key.E;
#else
    public KeyCode interactKey = KeyCode.E;
#endif

    [Header("Sound")]
    [Tooltip("Plays when the door is used. Leave empty for no sound.")]
    public AudioClip doorSound;
    [Range(0f, 1f)] public float volume = 1f;
    [Tooltip("Wait for the sound to finish before changing scene, so it isn't cut off.")]
    public bool waitForSound = true;
    [Tooltip("Optional. Add an Audio Source to control mixer group, 3D sound, etc. One is created if empty.")]
    public AudioSource audioSource;

    [Header("Hooks (optional)")]
    public UnityEvent onPlayerEnter;   // Press E
    public UnityEvent onPlayerExit;    // hide prompt
    public UnityEvent onUsed;          // Fade out

    public bool PlayerIsNear { get; private set; }
    bool loading;

    void Awake()
    {
        if (!audioSource) audioSource = GetComponent<AudioSource>();
        if (!audioSource)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
            audioSource.playOnAwake = false;
        }
    }

    void Update()
    {
        if (!PlayerIsNear || loading || loadOnEnter) return;
        if (InteractPressed()) Use();
    }

    bool InteractPressed()
    {
#if ENABLE_INPUT_SYSTEM
        return Keyboard.current != null && Keyboard.current[interactKey].wasPressedThisFrame;
#else
        return Input.GetKeyDown(interactKey);
#endif
    }

    public void Use()
    {
        if (loading) return;
        StartCoroutine(UseRoutine());
    }

    IEnumerator UseRoutine()
    {
        loading = true;
        onUsed?.Invoke();

        float wait = 0f;
        if (doorSound)
        {
            audioSource.PlayOneShot(doorSound, volume);
            if (waitForSound) wait = doorSound.length;
        }

        // for our sound designer, it still plays even without a  scene name for testing
        if (string.IsNullOrEmpty(sceneToLoad))
        {
            Debug.LogWarning($"{name}: no scene name set on SceneDoor.", this);
            yield return new WaitForSecondsRealtime(wait);
            loading = false;
            yield break;
        }

        if (wait > 0f) yield return new WaitForSecondsRealtime(wait);
        SceneManager.LoadScene(sceneToLoad);
    }

   
    bool IsPlayer(GameObject other)
    {
        for (var t = other.transform; t != null; t = t.parent)
            if (t.CompareTag(playerTag)) return true;
        return false;
    }

    void Enter(GameObject other)
    {
        if (!IsPlayer(other)) return;
        PlayerIsNear = true;
        onPlayerEnter?.Invoke();
        if (loadOnEnter) Use();
    }

    void Exit(GameObject other)
    {
        if (!IsPlayer(other)) return;
        PlayerIsNear = false;
        onPlayerExit?.Invoke();
    }

    void OnTriggerEnter(Collider other) => Enter(other.gameObject);
    void OnTriggerExit(Collider other) => Exit(other.gameObject);
    void OnTriggerEnter2D(Collider2D other) => Enter(other.gameObject);
    void OnTriggerExit2D(Collider2D other) => Exit(other.gameObject);
}