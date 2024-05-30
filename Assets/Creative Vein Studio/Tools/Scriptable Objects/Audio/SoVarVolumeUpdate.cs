using MAIN_PROJECT._Scripts.Enums;
using UnityEngine;

namespace Creative_Vein_Studio.Tools.Scriptable_Objects.Audio
{
    [CreateAssetMenu(fileName = "VolumeVar", menuName = "CVeinStudio/Audio/Variables/Volume")]
    public class SoVarVolumeUpdate : ScriptableObject
    {
        public EAudioChannelVariableName channelVarName;
        [Range(-80, 0)] public float volLevel = 0;
        [Range(0f, 1)] public float fadeDuration = 1;
    }
}