using System.Collections;
using UnityEngine;
using TMPro;

public class CountdownBubble : MonoBehaviour
{
    public TMP_Text text;
    public int startNumber = 10;
    public float secondsPerNumber = 1.5f;
    public bool loop = true;
    public string finalText = "Ready or not...";

    void OnEnable()
    {
        StartCoroutine(Count());
    }

    IEnumerator Count()
    {
        do
        {
            for (int i = startNumber; i >= 1; i--)
            {
                text.text = i + "…";
                yield return new WaitForSeconds(secondsPerNumber);
            }
        }
        while (loop);

        text.text = finalText;
    }
}