using UnityEngine;

public class TurnOnWhenPlayerEnters : MonoBehaviour
{
    public GameObject objectToTurnOn;

    private bool done = false;

    void OnTriggerEnter(Collider other)
    {
        if (done || !other.CompareTag("Player")) return;

        done = true;
        objectToTurnOn.SetActive(true);
    }
}