using System;
using _MAIN_APP.Scripts.ScriptableObjects;
using UnityEngine;

namespace _MAIN_APP.Scripts.Brokers
{
    [CreateAssetMenu(fileName = "BrokerUIActions", menuName = "TTM/Brokers/UIActions")]
    public class SoBrokerUiActions : ScriptableObject
    {
        public Action<int> OnSelectedExpansion;
        public Action<int> OnSelectedTrack;
        public Action<SoExpansionDetails> OnDownloadExpansion;

        public Action OnPlayClickEvent;
        public Action OnPlayConfirmEvent;
        public Action OnPlayCancelEvent;
        public Action OnPlayErrorEvent;
        public Action OnPlayPurchaseEvent;

        public void TriggerOnPlayClickEvent() => OnPlayClickEvent?.Invoke();
        public void TriggerOnPlayConfirmEvent() => OnPlayConfirmEvent?.Invoke();
        public void TriggerOnPlayCancelEvent() => OnPlayCancelEvent?.Invoke();
        public void TriggerOnPlayErrorEvent() => OnPlayErrorEvent?.Invoke();
        public void TriggerOnPlayPurchaseEvent() => OnPlayPurchaseEvent?.Invoke();

        public void TriggerOnDownloadExpansion(SoExpansionDetails val) => OnDownloadExpansion?.Invoke(val);

        public void TriggerOnSelectedABiome(int val) => OnSelectedExpansion?.Invoke(val);

        public void TriggerOnSelectedTrack(int val) => OnSelectedTrack?.Invoke(val);
    }
}