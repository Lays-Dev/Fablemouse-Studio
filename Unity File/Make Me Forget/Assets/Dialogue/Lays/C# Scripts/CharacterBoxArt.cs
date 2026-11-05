using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class CharacterBoxArt : MonoBehaviour
{
    public TMP_Text characterNameText;
    public Image boxBackground;
    public Sprite lonaBox;
    public Sprite jasonBox;

    void Update()
    {
        if (characterNameText.text == "Jason")
            boxBackground.sprite = jasonBox;
        else
            boxBackground.sprite = lonaBox;
    }
}