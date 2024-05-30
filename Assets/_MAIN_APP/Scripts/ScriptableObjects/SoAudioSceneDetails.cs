using System;
using _MAIN_APP.Scripts.Abstracts;
using _MAIN_APP.Scripts.Brokers;
using _MAIN_APP.Scripts.Interfaces;
using _MAIN_APP.Scripts.Models;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace _MAIN_APP.Scripts.ScriptableObjects
{
    [CreateAssetMenu(fileName = "_AudioScene", menuName = "TTM/AudioSceneDetails")]
    public class SoAudioSceneDetails : ScriptableObject
    {
        [SerializeField] private BrokerSoundBank _soundbankBroker;
        [SerializeField] internal UIDisplayData details;

        [SerializeField] internal AssetReference audioReference;
        public AssetReference AudioReference => audioReference;

        internal IAudioSceneActions _audioController;

        public void DestroyReference()
        {
            if (!AddressableManager.Instance) return;
            AddressableManager.Instance.UnLoadAndDestroy(audioReference);
        }

        public void UnloadReference()
        {
            _audioController.Unload();
        }

        public void CreateOrLoadReference()
        {
            if (_audioController != null)
            {
                _audioController.LoadAndPlay();
                return;
            }

            _audioController = AddressableManager.Instance.LoadAndInstantiate(audioReference)
                ?.GetComponent<AudioSceneController>();
            _audioController.LoadAndPlay();
        }
    }
}