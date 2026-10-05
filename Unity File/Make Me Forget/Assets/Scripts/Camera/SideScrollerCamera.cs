using UnityEngine;

[RequireComponent(typeof(Camera))]
public class SideScrollerCamera : MonoBehaviour
{
    [Header("Target")]
    public Transform target;

    [Header("Framing")]
    public float distance = 10f;
    public Vector2 offset = new Vector2(0f, 1.5f);

    [Header("Smoothing")]
    public float smoothTimeX = 0.25f;
    public float smoothTimeY = 0.4f;

    [Header("Dead Zone")]
    public Vector2 deadZoneSize = new Vector2(1f, 2f);

    [Header("Lookahead")]
    public float lookaheadDistance = 2f;
    public float lookaheadSmoothTime = 0.5f;
    public float lookaheadMinSpeed = 0.1f;

    [Header("Level Bounds")]
    public bool useBounds = false;
    public Vector2 boundsMin = new Vector2(-20f, -5f);
    public Vector2 boundsMax = new Vector2(20f, 10f);

    Camera cam;
    Vector3 focusPoint;
    Vector3 lastTargetPos;
    Vector2 currentLook;
    float facing = 1f;
    float currentLookahead;
    float lookaheadVelocity;
    float velX, velY;

    void Awake()
    {
        cam = GetComponent<Camera>();
    }

    void Start()
    {
        SnapToTarget();
    }

    public void SnapToTarget()
    {
        if (target == null) return;

        focusPoint = target.position;
        lastTargetPos = target.position;
        currentLookahead = facing * lookaheadDistance;
        lookaheadVelocity = 0f;
        velX = velY = 0f;

        currentLook = GetDesiredPosition();
        ApplyTransform(target.position.z);
    }

    void LateUpdate()
    {
        if (target == null)
        {
            GameObject player = GameObject.FindWithTag("Player");
            if (player == null) return;
            target = player.transform;
            SnapToTarget();
        }

        Vector3 t = target.position;

        float halfW = deadZoneSize.x * 0.5f;
        float halfH = deadZoneSize.y * 0.5f;

        if (t.x > focusPoint.x + halfW) focusPoint.x = t.x - halfW;
        else if (t.x < focusPoint.x - halfW) focusPoint.x = t.x + halfW;

        if (t.y > focusPoint.y + halfH) focusPoint.y = t.y - halfH;
        else if (t.y < focusPoint.y - halfH) focusPoint.y = t.y + halfH;

        if (Time.deltaTime > 0f)
        {
            float speedX = (t.x - lastTargetPos.x) / Time.deltaTime;
            if (Mathf.Abs(speedX) > lookaheadMinSpeed)
                facing = Mathf.Sign(speedX);
        }
        lastTargetPos = t;

        currentLookahead = Mathf.SmoothDamp(
            currentLookahead, facing * lookaheadDistance, ref lookaheadVelocity, lookaheadSmoothTime);

        Vector2 desired = GetDesiredPosition();
        currentLook.x = Mathf.SmoothDamp(currentLook.x, desired.x, ref velX, smoothTimeX);
        currentLook.y = Mathf.SmoothDamp(currentLook.y, desired.y, ref velY, smoothTimeY);

        ApplyTransform(t.z);
    }

    void ApplyTransform(float targetZ)
    {
        Quaternion rot = transform.rotation;
        Vector3 lookPoint = new Vector3(currentLook.x, currentLook.y, targetZ);

        transform.position = lookPoint - rot * Vector3.forward * distance;
    }

    Vector2 GetDesiredPosition()
    {
        float x = focusPoint.x + offset.x + currentLookahead;
        float y = focusPoint.y + offset.y;

        if (useBounds)
        {
            Vector2 half = GetHalfViewSize();
            x = ClampAxis(x, boundsMin.x, boundsMax.x, half.x);
            y = ClampAxis(y, boundsMin.y, boundsMax.y, half.y);
        }

        return new Vector2(x, y);
    }

    Vector2 GetHalfViewSize()
    {
        if (cam == null) cam = GetComponent<Camera>();

        float halfHeight = cam.orthographic
            ? cam.orthographicSize
            : distance * Mathf.Tan(cam.fieldOfView * 0.5f * Mathf.Deg2Rad);

        return new Vector2(halfHeight * cam.aspect, halfHeight);
    }

    static float ClampAxis(float value, float min, float max, float halfSize)
    {
        float lo = min + halfSize;
        float hi = max - halfSize;

        if (lo > hi) return (min + max) * 0.5f;

        return Mathf.Clamp(value, lo, hi);
    }

    void OnDrawGizmosSelected()
    {
        float z = target != null ? target.position.z : 0f;

        if (useBounds)
        {
            Gizmos.color = Color.yellow;
            Vector3 center = new Vector3((boundsMin.x + boundsMax.x) * 0.5f, (boundsMin.y + boundsMax.y) * 0.5f, z);
            Vector3 size = new Vector3(boundsMax.x - boundsMin.x, boundsMax.y - boundsMin.y, 0f);
            Gizmos.DrawWireCube(center, size);
        }

        if (target != null)
        {
            Gizmos.color = Color.green;
            Vector3 dzCenter = Application.isPlaying ? focusPoint : target.position;
            Gizmos.DrawWireCube(new Vector3(dzCenter.x, dzCenter.y, z), new Vector3(deadZoneSize.x, deadZoneSize.y, 0f));
        }
    }
}