using UnityEngine;
using UnityEngine.EventSystems;
using FMODUnity;

public class MoveRock : MonoBehaviour, IBeginDragHandler, IDragHandler
{
    // EventReference is a struct from FMOD integration
    [SerializeField] private EventReference rockDragSFX;

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
        RuntimeManager.PlayOneShot(rockDragSFX);
    }

    public void OnDrag(PointerEventData eventData)
    {
        // Move the rock with the pointer or touch
        rectTransform.anchoredPosition +=
            eventData.delta / canvas.scaleFactor;
    }
}

