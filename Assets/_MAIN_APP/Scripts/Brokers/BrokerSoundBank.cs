using System;
using _MAIN_APP.Scripts.Abstracts;
using _MAIN_APP.Scripts.Interfaces;
using UnityEngine;

namespace _MAIN_APP.Scripts.Brokers
{
    [CreateAssetMenu(fileName = "Broker_SoundBank", menuName = "TTM/Brokers/SoundBanks")]
    public class BrokerSoundBank : ScriptableObject
    {
        public static Action<IAudioSceneActions> OnSoundBankLoaded;
        public static Action OnPlaySoundBank;
        public static Action OnPauseSoundBank;
        public static Action OnResumeSoundBank;
        public static Action OnStopSoundBank;

        public void TriggerOnSoundBankLoaded(IAudioSceneActions val)
        {
            OnSoundBankLoaded?.Invoke(val);
        }

        public void TriggerOnPlaySoundBank()
        {
            OnPlaySoundBank?.Invoke();
        }

        public void TriggerOnPauseSoundBank()
        {
            OnPauseSoundBank?.Invoke();
        }

        public void TriggerOnResumeSoundBank()
        {
            OnResumeSoundBank?.Invoke();
        }

        public void TriggerOnStopSoundBank()
        {
            OnStopSoundBank?.Invoke();
        }
    }
}