
using System;
using UnityEngine;

public class SaveableObject : MonoBehaviour, ISaveable
{
    [Header("Save Settings")]
    [SerializeField] private string _saveId;

    [SerializeField] private bool _savePosition = true;
    [SerializeField] private bool _saveRotation = true;
    [SerializeField] private bool _saveScale = false;
    [SerializeField] private bool _saveActiveState = true;

    public string SaveId => _saveId;

    [Serializable]
    private class ObjectState
    {
        public Vector3 position;
        public Quaternion rotation;
        public Vector3 scale;
        public bool isActive;
    }

    public string CaptureState()
    {
        ObjectState state = new ObjectState();

        state.position = transform.position;
        state.rotation = transform.rotation;
        state.scale = transform.localScale;
        state.isActive = gameObject.activeSelf;

        return JsonUtility.ToJson(state);
    }

    public void RestoreState(string json)
    {
        ObjectState state = JsonUtility.FromJson<ObjectState>(json);

        if (_savePosition)
            transform.position = state.position;

        if (_saveRotation)
            transform.rotation = state.rotation;

        if (_saveScale)
            transform.localScale = state.scale;

        if (_saveActiveState)
            gameObject.SetActive(state.isActive);
    }
}

