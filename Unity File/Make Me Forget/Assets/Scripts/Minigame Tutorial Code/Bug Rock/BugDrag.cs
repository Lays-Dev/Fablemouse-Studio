using UnityEngine;
using UnityEngine.EventSystems;

public class BugDrag : MonoBehaviour, IBeginDragHandler, IDragHandler
{
    private RectTransform rectTransform;
    private Canvas canvas;

    private void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        canvas = GetComponentInParent<Canvas>();
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        // Start dragging the rock
    }

    public void OnDrag(PointerEventData eventData)
    {
        // Move the rock with the pointer or touch
        rectTransform.anchoredPosition +=
            eventData.delta / canvas.scaleFactor;
    }
}
