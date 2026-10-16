using UnityEngine;
using UnityEngine.EventSystems;

// IBeginDragHandler and IDragHandler. Interfaces that let Unity detect when a player moves or drags a UI GameObject.
// IBeginDragHandler runs when the player begins dragging.
// IDragHandler runs while the player moves the GameObject.

public class DragRock : MonoBehaviour, IBeginDragHandler, IDragHandler
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
