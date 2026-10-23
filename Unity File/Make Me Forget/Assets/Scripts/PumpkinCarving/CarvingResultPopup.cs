using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

// goes on CarvingManager pls
public class CarvingResultPopup : MonoBehaviour
{
    public PumpkinCarvingMinigame minigame;

    [Header("Popup")]
    public GameObject popupPanel;
    public GameObject winContent;
    public GameObject loseContent;

    [Header("Score")]
    public TMP_Text accuracyText;
    public GameObject[] starIcons;

    [Header("Buttons")]
    public Button tryAgainButton;
    public Button leaveButton;

    public float delay = 1f; // so u can see the pumpkin glow first

    void Awake()
    {
        if (!minigame) minigame = GetComponent<PumpkinCarvingMinigame>();

        popupPanel.SetActive(false);
        minigame.onFinished.AddListener(r => StartCoroutine(Show(r)));
        minigame.onStarted.AddListener(Hide);

        // buttons hooked up here, dont add them in On Click too or it runs twice
        tryAgainButton.onClick.AddListener(TryAgain);
        leaveButton.onClick.AddListener(Leave);
    }

    IEnumerator Show(CarvingResult r)
    {
        yield return new WaitForSecondsRealtime(delay);
        if (minigame.Running) yield break;

        bool won = minigame.Won;
        if (winContent) winContent.SetActive(won);
        if (loseContent) loseContent.SetActive(!won);
        if (accuracyText) accuracyText.text = $"Accuracy {r.Percent}%";

        // turn on 1 star per star earned
        if (starIcons != null)
            for (int i = 0; i < starIcons.Length; i++)
                if (starIcons[i]) starIcons[i].SetActive(i < r.stars);

        popupPanel.SetActive(true);
    }

    public void Hide()
    {
        StopAllCoroutines();
        popupPanel.SetActive(false);
    }

    public void TryAgain()
    {
        Hide();
        minigame.Retry();
    }

    public void Leave()
    {
        Hide();
        minigame.Close();
    }
}