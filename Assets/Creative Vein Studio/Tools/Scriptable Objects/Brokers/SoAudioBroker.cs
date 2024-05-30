using System;
using System.Collections.Generic;
using Creative_Vein_Studio.Tools.Scriptable_Objects.Audio;
using CVStudio;
using MAIN_PROJECT._Scripts.Enums;
using UnityEngine;
using UnityEngine.Events;

namespace _MainApp.Scripts.Brokers
{
    [CreateAssetMenu(fileName = "AudioBroker", menuName = "CVeinStudio/Audio/Broker")]
    public class SoAudioBroker : ScriptableObject
    {
        public Action onPlayButtonSuccess;
        public Action onPlayButtonCancel;
        public Action<EAudioChannelVariableName, float, float> onVolumeUpdate;
        public Action<List<AudioClip>, EAudioChannelVariableName> onPlayClipOnChannel;

        public void TriggerPlayButtonSuccess()
        {
            onPlayButtonSuccess?.Invoke();
        }

        public void TriggerPlayButtonCancel()
        {
            onPlayButtonCancel?.Invoke();
        }

        public void TriggerUpdateVolume(SoVarVolumeUpdate val)
        {
            onVolumeUpdate?.Invoke(val.channelVarName, val.volLevel, val.fadeDuration);
        }

        public void TriggerPlayAudioClipOnChannel(SoAudioClipInfo val)
        {
            onPlayClipOnChannel?.Invoke(val.Clips, val.channelVariableName);
        }
    }
}