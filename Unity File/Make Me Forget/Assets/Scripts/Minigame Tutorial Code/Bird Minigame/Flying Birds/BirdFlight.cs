using UnityEngine;
using System.Collections;

public class BirdFlight : MonoBehaviour
{
    [Header("Flight Settings")]

    // The Transform point the birds will fly toward.
    public RectTransform flightPoint;

    // How long the birds take to fly to the transform point.
    public float flightDuration = 2f;

    // Delay before this birds starts flying.
    public float startDelay = 0f;

    // Store the birds RectTransform.
    private RectTransform rectTransform;

    private void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
    }

    // Call this function to make the birds fly away.
    public void FlyAway()
    {
        StartCoroutine(FlyCoroutine());
    }

    IEnumerator FlyCoroutine()
    {
        // Wait before starting the flight.
        if (startDelay > 0)
        {
            yield return new WaitForSeconds(startDelay);
        }

        // Remember the starting position.
        Vector2 startPosition = rectTransform.anchoredPosition;

        // Get the transform point posistion.
        Vector2 targetPosition = flightPoint.anchoredPosition;

        float elapsedTime = 0f;

        // Move the birds toward the target.
        while (elapsedTime < flightDuration)
        {
            elapsedTime += Time.deltaTime;

            float progress = elapsedTime / flightDuration;

            rectTransform.anchoredPosition = Vector2.Lerp(
                startPosition,
                targetPosition,
                progress
            );

            yield return null;
        }

        // Make sure the birds reach the transform point.
        rectTransform.anchoredPosition = targetPosition;

        // Hide the birds after flying away.
        gameObject.SetActive(false);
    }
}
