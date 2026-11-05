using UnityEngine;

// When this object turns on, it also turns on another object.
// Put it on Temp Bubble so the rock's DialogueZone
// only turns on after the eggs hatch.
public class TurnOnWhenEnabled : MonoBehaviour
{
    public GameObject objectToTurnOn;

    void OnEnable()
    {
        objectToTurnOn.SetActive(true);
    }
}