using UnityEngine;
using UnityEngine.UI;
using FMODUnity;

[RequireComponent(typeof(Button))]
public class ButtonSFX : MonoBehaviour
{
    [SerializeField] private EventReference _clickSound;

    private Button _button;

    private void Awake()
    {
        _button = GetComponent<Button>();
        _button.onClick.AddListener(PlaySound);
    }

    private void PlaySound()
    {
        if (!_clickSound.IsNull)
        {
            RuntimeManager.PlayOneShot(_clickSound);
        }
    }

    private void OnDestroy()
    {
        _button.onClick.RemoveListener(PlaySound);
    }
}
