using UnityEngine;
using UnityEngine.InputSystem;

// slap this on the MAIN CAMERA
// it only zooms the lens, side scroller camera stays untouched hehe
public class CameraZoom : MonoBehaviour
{
    [Header("Limits (1 = normal view)")]
    // how close u can get, smaller = more zoomed in
    public float zoomInLimit = 0.7f;
    // how far out u can go, just a lil bit
    public float zoomOutLimit = 1.15f;

    [Header("Feel")]
    // how much one scroll click zooms
    public float scrollStep = 0.05f;
    // how fast it glides there
    public float smoothSpeed = 8f;

    private Camera cam;
    private float startView;
    private float targetZoom = 1f;
    private float currentZoom = 1f;

    private void Start()
    {
        cam = GetComponent<Camera>();

        // remember the normal view so we know where "1" is
        startView = cam.orthographic ? cam.orthographicSize : cam.fieldOfView;
    }

    private void Update()
    {
        // scroll wheel, up = closer to lona, down = back out
        if (Mouse.current != null)
        {
            float scroll = Mouse.current.scroll.ReadValue().y;
            if (scroll != 0f)
            {
                targetZoom -= Mathf.Sign(scroll) * scrollStep;
            }
        }

        // controller peeps, click the right stick to zoom in, click again to go back
        if (Gamepad.current != null && Gamepad.current.rightStickButton.wasPressedThisFrame)
        {
            if (targetZoom < 1f) targetZoom = 1f;
            else targetZoom = zoomInLimit;
        }

        // no zoomin' past the limits 
        targetZoom = Mathf.Clamp(targetZoom, zoomInLimit, zoomOutLimit);

        // smooth glide instead of snapping
        currentZoom = Mathf.Lerp(currentZoom, targetZoom, smoothSpeed * Time.deltaTime);

        // actually zoom the lens
        if (cam.orthographic) cam.orthographicSize = startView * currentZoom;
        else cam.fieldOfView = startView * currentZoom;
    }
}