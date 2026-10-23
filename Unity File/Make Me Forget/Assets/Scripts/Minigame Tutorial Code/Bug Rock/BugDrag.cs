using UnityEngine;
using UnityEngine.EventSystems;
using FMODUnity;
using FMOD.Studio;

public class BugDrag : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    [SerializeField] private EventReference bugDragSFX;
    [SerializeField] private EventReference bugsCrawlSFX;

    private EventInstance bugBeingDraggedSFX;

    private RectTransform rectTransform;
    private Canvas canvas;

    private void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        canvas = GetComponentInParent<Canvas>();
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        // Start dragging the bug
        RuntimeManager.PlayOneShot(bugDragSFX);
        bugBeingDraggedSFX = RuntimeManager.CreateInstance(bugsCrawlSFX);
        bugBeingDraggedSFX.start();
    }

    public void OnDrag(PointerEventData eventData)
    {
        rectTransform.anchoredPosition += eventData.delta / canvas.scaleFactor;

        float speed = eventData.delta.magnitude;
        bugBeingDraggedSFX.setParameterByName("DragSpeed", speed);
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        StopLoop();
    }

    private void StopLoop()
    {
        if (!bugBeingDraggedSFX.isValid()) return;

        bugBeingDraggedSFX.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
        bugBeingDraggedSFX.release();
        bugBeingDraggedSFX.clearHandle();
    }

    private void OnDestroy()
    {
        StopLoop();
    }
}
