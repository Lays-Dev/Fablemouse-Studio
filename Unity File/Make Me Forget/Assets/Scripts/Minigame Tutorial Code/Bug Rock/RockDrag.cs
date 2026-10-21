using UnityEngine;
using UnityEngine.EventSystems;

public class RockDrag : MonoBehaviour, IDragHandler
{
    [SerializeField] private RectTransform _rockImage;
    [SerializeField] private GameObject _collectibleBug;
    [SerializeField] private float _distanceNeeded = 150f;

    private Vector2 _startPosition;
    private bool _moved;

    void Start()
    {
        // remember where the rock started
        _startPosition = _rockImage.anchoredPosition;

        // hide the bug until the rock moves
        if (_collectibleBug != null)
            _collectibleBug.SetActive(false);
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (_moved) return;

        RectTransform parent = _rockImage.parent as RectTransform;

        // move the rock with the mouse
        if (RectTransformUtility.ScreenPointToLocalPointInRectangle(
            parent, eventData.position, eventData.pressEventCamera,
            out Vector2 position))
        {
            _rockImage.anchoredPosition = position;
        }

        // check if the rock moved far enough
        float distance = Vector2.Distance(
            _startPosition, _rockImage.anchoredPosition);

        if (distance >= _distanceNeeded)
        {
            _moved = true;

            // reveal the bug
            if (_collectibleBug != null)
                _collectibleBug.SetActive(true);

            Debug.Log("Rock moved, bug revealed!");
        }
    }
}
