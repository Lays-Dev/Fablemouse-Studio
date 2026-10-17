using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class BugClick : MonoBehaviour
{
   public Sprite clickedBugImage;

    private Image bugImage;

    void Awake()
    {
        bugImage = GetComponent<Image>();
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (bugImage != null && clickedBugImage != null)
        {
            bugImage.sprite = clickedBugImage;
        }
    }
}
