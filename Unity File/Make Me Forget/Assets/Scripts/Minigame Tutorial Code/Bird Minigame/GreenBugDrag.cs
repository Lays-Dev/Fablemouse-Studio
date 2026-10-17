
using UnityEngine;
using UnityEngine.EventSystems;

public class GreenBugDrag : MonoBehaviour,
    IBeginDragHandler, IDragHandler, IEndDragHandler
{
    private RectTransform rectTransform;
    private Canvas canvas;

    // Store the bug's starting position.
    private Vector2 startPosition;

    // Assign the bird GameObject in the Inspector.
    public GameObject bird;
    public GameObject heldItemCanvas;
    public GameObject dialogueImage;
    public GameObject birdsHungry;
    public GameObject birdsFlight;

    private void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        canvas = GetComponentInParent<Canvas>();

        // Remember where the bug starts.
        startPosition = rectTransform.anchoredPosition;
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        Debug.Log("Started dragging the bug!");
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (canvas != null)
        {
            rectTransform.anchoredPosition +=
                eventData.delta / canvas.scaleFactor;
        }
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        // Check if the bug was dropped on the bird.
        if (IsOverBird())
        {
            Debug.Log("Bug fed to the birds!");
            dialogueImage.gameObject.SetActive(false);
            birdsHungry.gameObject.SetActive(false);
            birdsFlight.gameObject.SetActive(true);

            // Hide the bug after feeding.
            heldItemCanvas.gameObject.SetActive(false);


        }
        else
        {
            // Return the bug to its starting position.
            rectTransform.anchoredPosition = startPosition;

            Debug.Log("Bug returned to starting position.");
        }
    }

    private bool IsOverBird()
    {
        // Only allow feeding if the bird is active.
        if (bird == null || !bird.activeInHierarchy)
            return false;

        RectTransform birdRect = bird.GetComponent<RectTransform>();

        if (birdRect != null)
        {
            // Check whether the bug overlaps the bird's UI rectangle.
            return RectTransformUtility.RectangleContainsScreenPoint(
                birdRect,
                RectTransformUtility.WorldToScreenPoint(
                    canvas.worldCamera,
                    rectTransform.position),
                canvas.renderMode == RenderMode.ScreenSpaceOverlay
                    ? null
                    : canvas.worldCamera
            );
        }

        // For a non-UI bird GameObject, check its Collider.
        Collider2D birdCollider = bird.GetComponent<Collider2D>();

        if (birdCollider != null)
        {
            Vector2 bugPosition = rectTransform.position;
            return birdCollider.OverlapPoint(bugPosition);
        }

        return false;
    }
}
