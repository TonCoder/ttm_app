using System;
using _MAIN_APP.Scripts.Models;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace _MAIN_APP.Scripts.ScriptableObjects
{
    [CreateAssetMenu(fileName = "_AudioTrack", menuName = "TTM/AudioTrackDetails")]
    public class SoAudioTrackDetails : ScriptableObject
    {
        [SerializeField] internal UIDisplayData details;

        [SerializeField] private AssetReference audioReference;
        public AssetReference AudioReference => audioReference;
    }
}