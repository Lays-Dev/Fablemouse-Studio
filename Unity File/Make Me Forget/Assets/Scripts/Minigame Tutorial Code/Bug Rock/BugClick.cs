
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class BugClick : MonoBehaviour, IPointerClickHandler
{
    public Sprite clickedBugImage;

    private Image bugImage;

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
            bugImage.sprite = clickedBugImage;
            Debug.Log("Bug image changed!");
        }
        else
        {
            Debug.LogWarning("Missing Image component or Clicked Bug Image sprite!");
        }
    }
}
