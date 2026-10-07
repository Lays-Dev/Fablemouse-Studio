using UnityEngine;

public class DialogueBoxFollow : MonoBehaviour
{
    // Character this dialogue box follows.
    [SerializeField] private Transform _target;

    // Position above the character's head.
    [SerializeField] private Vector3 _offset = new Vector3(0f, 2f, 0f);

    // Camera rendering the game.
    [SerializeField] private Camera _camera;

    // Keeps the UI positioned above the target character.
    private void LateUpdate()
    {
        if (_target == null || _camera == null)
        {
            return;
        }

        Vector3 targetPosition = _target.position + _offset;

        transform.position = _camera.WorldToScreenPoint(targetPosition);
    }
}
