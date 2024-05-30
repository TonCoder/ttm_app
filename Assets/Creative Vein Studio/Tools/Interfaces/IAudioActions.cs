using System.Collections.Generic;
using Creative_Vein_Studio.Tools.Scriptable_Objects.Audio;
using MAIN_PROJECT._Scripts.Enums;
using UnityEngine;

namespace Creative_Vein_Studio.Tools.Interfaces
{
    public interface IAudioActions
    {
        void PlayButtonSuccess();
        void PlayButtonCancel();
        void PlaySfx(SoAudioClipInfo soAudioClipInfo);
        void UpdateVolume(EAudioChannelVariableName channelVarName, float volLevel = 1, float fadeDuration = 1);
        void PlayAudioClipOnChannel(List<AudioClip> clip, EAudioChannelVariableName channelVariableName);
    }
}