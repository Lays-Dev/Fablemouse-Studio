using UnityEngine;
using UnityEngine.EventSystems;

public class BranchDrag : MonoBehaviour, IDragHandler, IPointerDownHandler
{
    private RectTransform _rectTransform;
    private Canvas _canvas;

    void Awake()
    {
        _rectTransform = GetComponent<RectTransform>();
        
        // Automatically finds the parent Canvas to handle scaling
        _canvas = GetComponentInParent<Canvas>();
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        // Optional: Brings the item to the front layer when clicked
        _rectTransform.SetAsLastSibling();
    }

    public void OnDrag(PointerEventData eventData)
    {
        // Moves the UI element accurately using the New Input System's data
        _rectTransform.anchoredPosition -= eventData.delta / _canvas.scaleFactor;
    }
}
