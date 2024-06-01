using System;
using _MAIN_APP.Scripts.Models;
using _MAIN_APP.Scripts.ScriptableObjects;

namespace _MAIN_APP.Scripts.Brokers
{
    public class BrokerUiActions
    {
        public static Action<int> OnSelectedExpansion;
        public static Action<int> OnSelectedTrack;
        public static Action<SoExpansionDetails> OnDownloadExpansion;

        public static void TriggerOnDownloadExpansion(SoExpansionDetails val) => OnDownloadExpansion?.Invoke(val);

        public static void TriggerOnSelectedABiome(int val) => OnSelectedExpansion?.Invoke(val);

        public static void TriggerOnSelectedTrack(int val) => OnSelectedTrack?.Invoke(val);
    }
}