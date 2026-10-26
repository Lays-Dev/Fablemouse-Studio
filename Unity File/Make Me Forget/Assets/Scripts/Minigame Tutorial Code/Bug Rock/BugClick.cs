
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using FMODUnity;

public class BugClick : MonoBehaviour, IPointerClickHandler
{
    [SerializeField] private EventReference bugSplatSFX;

    public Sprite clickedBugImage;

    private Image bugImage;

    private bool _isSquished = false;

    public bool IsSquished => _isSquished;

    public void RestoreSquished(bool wasSquished)
    {
        // Restore whether this particular bug was squished.
        _isSquished = wasSquished;

        // Change the sprite without replaying the sound.
        if (_isSquished && bugImage != null && clickedBugImage != null)
        {
            bugImage.sprite = clickedBugImage;
        }
    }
    void Awake()
    {
        bugImage = GetComponent<Image>();

        Debug.Log("BugClick started on: " + gameObject.name);

        if (bugImage == null)
        {
            Debug.LogError("No Image component found on " + gameObject.name);
        }
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        Debug.Log("Bug clicked: " + gameObject.name);

        if (bugImage != null && clickedBugImage != null)
        {
            RuntimeManager.PlayOneShot(bugSplatSFX);
            bugImage.sprite = clickedBugImage;
            _isSquished = true;
            Debug.Log("Bug image changed!");
        }
        else
        {
            Debug.LogWarning("Missing Image component or Clicked Bug Image sprite!");
        }
    }
}
