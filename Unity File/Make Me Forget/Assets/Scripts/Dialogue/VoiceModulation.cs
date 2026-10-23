using System.Collections;
using UnityEngine;
using FMODUnity;
using FMOD.Studio;

public class VoiceModulation : MonoBehaviour
{
    [Header("Yarn Speaker")]
    [SerializeField] private string speakerName;

    [Header("FMOD")]
    [SerializeField] private EventReference voiceEvent;

    [Header("Speech Timing")]
    [SerializeField, Min(0.01f)] private float pulseInterval = 0.085f;
    [SerializeField, Min(0.01f)] private float minPulseInterval = 0.045f;
    [SerializeField, Min(0.01f)] private float maxPulseInterval = 0.13f;
    [SerializeField, Range(0f, 1f)] private float punctuationPause = 0.3f;
    [SerializeField, Range(0f, 1f)] private float restChance = 0.08f;

    [Header("FMOD Modulation")]
    [SerializeField] private string pitchParameter = "Pitch";
    [SerializeField] private string intensityParameter = "Intensity";
    [SerializeField] private string vowelParameter = "Vowel";

    [SerializeField] private Vector2 pitchRange =
        new Vector2(-0.15f, 0.15f);

    [SerializeField] private Vector2 intensityRange =
        new Vector2(0.35f, 0.8f);

    [SerializeField] private Vector2 vowelRange =
        new Vector2(0.2f, 0.8f);

    [Header("Character Variation")]
    [SerializeField, Range(0f, 1f)]
    private float expressiveVariation = 0.5f;

    private Coroutine speechRoutine;

    public string SpeakerName => speakerName;

    public void Speak(string text)
    {
        StopSpeaking();

        if (string.IsNullOrEmpty(text) || voiceEvent.IsNull)
            return;

        speechRoutine = StartCoroutine(SpeakRoutine(text));
    }

    public void StopSpeaking()
    {
        if (speechRoutine != null)
        {
            StopCoroutine(speechRoutine);
            speechRoutine = null;
        }
    }

    private IEnumerator SpeakRoutine(string text)
    {
        foreach (char character in text)
        {
            if (char.IsWhiteSpace(character))
            {
                yield return new WaitForSeconds(
                    pulseInterval * 1.8f);
                continue;
            }

            if (IsPunctuation(character))
            {
                yield return new WaitForSeconds(
                    pulseInterval *
                    (1f + punctuationPause * 5f));
                continue;
            }

            // Skip non-alphanumeric symbols.
            if (!char.IsLetterOrDigit(character))
                continue;

            // Occasionally leave a small expressive rest.
            if (Random.value < restChance)
            {
                yield return new WaitForSeconds(
                    pulseInterval * Random.Range(1.2f, 2.5f));
            }

            PlayPulse(character);

            float variation = Random.Range(
                -expressiveVariation,
                expressiveVariation);

            float interval = Mathf.Clamp(
                pulseInterval * (1f + variation * 0.35f),
                minPulseInterval,
                maxPulseInterval);

            yield return new WaitForSeconds(interval);
        }

        speechRoutine = null;
    }

    private void PlayPulse(char character)
    {
        EventInstance instance =
            RuntimeManager.CreateInstance(voiceEvent);

        float pitch = Random.Range(
            pitchRange.x, pitchRange.y);

        float intensity = Random.Range(
            intensityRange.x, intensityRange.y);

        // Bias timbre slightly based on the text character.
        // This is a stylized modulation, not phonetic analysis.
        float vowel = IsVowel(character)
            ? Random.Range(0.55f, vowelRange.y)
            : Random.Range(vowelRange.x, 0.45f);

        instance.setParameterByName(pitchParameter, pitch);
        instance.setParameterByName(intensityParameter, intensity);
        instance.setParameterByName(vowelParameter, vowel);

        instance.set3DAttributes(
            RuntimeUtils.To3DAttributes(gameObject));

        instance.start();
        instance.release();
    }

    private bool IsPunctuation(char c)
    {
        return c == '.' || c == ',' || c == '!' ||
               c == '?' || c == ';' || c == ':';
    }

    private bool IsVowel(char c)
    {
        return "aeiouAEIOU".IndexOf(c) >= 0;
    }

    private void OnDisable()
    {
        StopSpeaking();
    }
}
