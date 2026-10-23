using System;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using Yarn.Unity;

public class YarnVoModulationView : DialoguePresenterBase
{
    [Header("Character Voice Profiles")]
    [SerializeField]
    private List<VoiceModulation> voiceProfiles =
        new List<VoiceModulation>();

    private readonly Dictionary<string, VoiceModulation> voices =
        new Dictionary<string, VoiceModulation>(
            StringComparer.OrdinalIgnoreCase);

    private VoiceModulation currentVoice;
    private CancellationTokenRegistration lineCancellation;

    private void Awake()
    {
        voices.Clear();

        foreach (VoiceModulation voice in voiceProfiles)
        {
            if (voice == null ||
                string.IsNullOrWhiteSpace(voice.SpeakerName))
            {
                continue;
            }

            if (voices.ContainsKey(voice.SpeakerName))
            {
                Debug.LogWarning(
                    $"Duplicate Yarn speaker name: {voice.SpeakerName}",
                    voice);
                continue;
            }

            voices.Add(voice.SpeakerName, voice);
        }
    }

    public override YarnTask RunLineAsync(
        LocalizedLine line,
        LineCancellationToken token)
    {
        // End any previous line's sound and cancellation callback.
        lineCancellation.Dispose();
        StopCurrentVoice();

        string speaker = line.CharacterName;
        string text = line.Text.Text;

        if (!string.IsNullOrWhiteSpace(speaker) &&
            voices.TryGetValue(speaker, out VoiceModulation voice))
        {
            currentVoice = voice;
            currentVoice.Speak(text);

            // Stop the sound when Yarn requests that this line end.
            lineCancellation = token.NextContentToken.Register(
                StopCurrentVoice);
        }

        // The regular Yarn LinePresenter controls text and line timing.
        return YarnTask.CompletedTask;
    }

    public override YarnTask OnDialogueStartedAsync()
    {
        lineCancellation.Dispose();
        StopCurrentVoice();

        return YarnTask.CompletedTask;
    }

    public override YarnTask OnDialogueCompleteAsync()
    {
        lineCancellation.Dispose();
        StopCurrentVoice();

        // Also clean up any voice profiles left running.
        foreach (VoiceModulation voice in voiceProfiles)
        {
            if (voice != null)
                voice.StopSpeaking();
        }

        return YarnTask.CompletedTask;
    }

    private void StopCurrentVoice()
    {
        if (currentVoice != null)
        {
            currentVoice.StopSpeaking();
            currentVoice = null;
        }
    }

    private void OnDisable()
    {
        lineCancellation.Dispose();
        StopCurrentVoice();
    }
}
