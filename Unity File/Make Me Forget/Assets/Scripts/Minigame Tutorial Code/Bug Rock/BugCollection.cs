using UnityEngine;
using UnityEngine.UI;

public class BugCollection : MonoBehaviour
{
    [SerializeField] private Button _bugButton;
    [SerializeField] private GameObject _inventoryIcon;
    [SerializeField] private MiniGamePopup _popup;

    private bool _collected;

    void Start()
    {
        // hide the inventory bug at first
        if (_inventoryIcon != null)
            _inventoryIcon.SetActive(false);

        // clicking the bug collects it
        if (_bugButton != null)
            _bugButton.onClick.AddListener(CollectBug);
    }

    void OnDestroy()
    {
        // clean up the button listener
        if (_bugButton != null)
            _bugButton.onClick.RemoveListener(CollectBug);
    }

    public void CollectBug()
    {
        if (_collected) return;

        _collected = true;

        // remove the bug from the popup
        if (_bugButton != null)
            _bugButton.gameObject.SetActive(false);

        // show it in the corner inventory
        if (_inventoryIcon != null)
            _inventoryIcon.SetActive(true);

        // tell the existing minigame system we're done
        if (_popup != null)
            _popup.Complete();

        Debug.Log("Collected the bug!");
    }
}
