using System;
using UnityEngine;

public class TurnOnWhenPlayerEnters : MonoBehaviour, ISaveable
{
    public GameObject objectToTurnOn;

    private bool done = false;

    // Name + position, so each trigger gets its own id.
    public string SaveId
    {
        get
        {
            Vector3 p = transform.position;
            return $"TurnOn_{gameObject.name}_{Mathf.RoundToInt(p.x * 10)}_{Mathf.RoundToInt(p.y * 10)}_{Mathf.RoundToInt(p.z * 10)}";
        }
    }

    [Serializable]
    private class State
    {
        public bool done;
    }

    public string CaptureState()
    {
        return JsonUtility.ToJson(new State { done = done });
    }

    public void RestoreState(string json)
    {
        done = JsonUtility.FromJson<State>(json).done;
    }

    void OnTriggerEnter(Collider other)
    {
        if (done || !other.CompareTag("Player")) return;

        done = true;
        objectToTurnOn.SetActive(true);
    }
}