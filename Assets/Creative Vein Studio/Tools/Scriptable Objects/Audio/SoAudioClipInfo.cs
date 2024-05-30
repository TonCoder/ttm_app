using System.Collections.Generic;
using CVStudio;
using MAIN_PROJECT._Scripts.Enums;
using UnityEngine;

namespace Creative_Vein_Studio.Tools.Scriptable_Objects.Audio
{
    [CreateAssetMenu(fileName = "AudioClip", menuName = "CVeinStudio/Audio/Variables/AudioClip")]
    public class SoAudioClipInfo : ScriptableObject
    {
        [SerializeField] public EAudioChannelVariableName channelVariableName;
        [SerializeField] private List<AudioClip> soundClip = new List<AudioClip>();
        [SerializeField, Range(0f, 1f)] private float soundVolume = 0.9f;

        [SerializeField] private Vector2 pitch = Vector2.one;

        public float Volume => soundVolume;
        public Vector2 Pitch => pitch;
        public List<AudioClip> Clips => soundClip;
        public EAudioChannelVariableName ChannelVariable => channelVariableName;
    }
}