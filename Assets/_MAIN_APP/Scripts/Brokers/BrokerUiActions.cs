using System;
using _MAIN_APP.Scripts.Models;
using _MAIN_APP.Scripts.ScriptableObjects;

namespace _MAIN_APP.Scripts.Brokers
{
    public class BrokerUiActions
    {
        public static Action<int> OnSelectedABiome;
        public static Action<int> OnSelectedAScene;
        public static Action<SoBiomeDetails> OnDownloadNewBiome;

        public static void TriggerOnDownloadNewBiome(SoBiomeDetails val) => OnDownloadNewBiome?.Invoke(val);

        public static void TriggerOnSelectedABiome(int val) => OnSelectedABiome?.Invoke(val);

        public static void TriggerOnSelectedAScene(int val) => OnSelectedAScene?.Invoke(val);
    }
}