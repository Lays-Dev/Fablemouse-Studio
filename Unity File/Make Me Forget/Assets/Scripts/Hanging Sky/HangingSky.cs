
using UnityEngine;

public class HangingSky: MonoBehaviour
{
    public float swingAngle = 5f;
    public float swingSpeed = 1f;

    private Quaternion startRotation;

    void Start()
    {
        startRotation = transform.localRotation;
    }

    void Update()
    {
        float angle = Mathf.Sin(Time.time * swingSpeed)
                      * swingAngle;

        transform.localRotation = startRotation *
            Quaternion.Euler(0f, 0f, angle);
    }
}
