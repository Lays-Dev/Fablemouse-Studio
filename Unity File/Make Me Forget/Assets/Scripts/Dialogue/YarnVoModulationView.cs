using System;
using System.Collections;
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

    [Header("Voice Timing")]
    [SerializeField, Min(0.1f)] private float _maxTalkingDuration = 1.5f;

    private readonly Dictionary<string, VoiceModulation> voices =
        new Dictionary<string, VoiceModulation>(
            StringComparer.OrdinalIgnoreCase);

    private VoiceModulation currentVoice;
    private CancellationTokenRegistration lineCancellation;
    private Coroutine _voiceTimer;

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
        lineCancellation.Dispose();
        StopCurrentVoice();

        string speaker = line.CharacterName;
        string text = line.Text.Text;

        if (!string.IsNullOrWhiteSpace(speaker) &&
            voices.TryGetValue(speaker, out VoiceModulation voice))
        {
            currentVoice = voice;
            currentVoice.Speak(text);

            // Stop talking when Yarn advances to the next line.
            lineCancellation = token.NextContentToken.Register(StopCurrentVoice);

            // Also stop talking after the maximum duration.
            _voiceTimer = StartCoroutine(StopVoiceAfterDelay());
        }

        return YarnTask.CompletedTask;
    }

    private IEnumerator StopVoiceAfterDelay()
    {
        yield return new WaitForSeconds(_maxTalkingDuration);
        StopCurrentVoice();
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
        if (_voiceTimer != null)
        {
            StopCoroutine(_voiceTimer);
            _voiceTimer = null;
        }

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
