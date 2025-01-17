using System;
using _MAIN_APP.Scripts.ScriptableObjects;
using UnityEngine;

namespace _MAIN_APP.Scripts.Brokers
{
    [CreateAssetMenu(fileName = "BrokerUIActions", menuName = "TTM/Brokers/UIActions")]
    public class SoBrokerUiActions : ScriptableObject
    {
        public Action<int> OnSelectedExpansion;
        public Action<int> OnDeleteExpansion;
        public Action<int> OnSelectedTrack;
        public Action<SoExpansionDetails> OnDownloadExpansion;
        public Action OnResetStore;

        public Action OnPlayClickEvent;
        public Action OnPlayConfirmEvent;
        public Action OnPlayCancelEvent;
        public Action OnPlayErrorEvent;
        public Action OnPlayPurchaseEvent;
        public Action OnPlayLogoEvent;

        public void TriggerPlayLogoEvent() => OnPlayLogoEvent?.Invoke();
        public void TriggerOnPlayClickEvent() => OnPlayClickEvent?.Invoke();
        public void TriggerOnPlayConfirmEvent() => OnPlayConfirmEvent?.Invoke();
        public void TriggerOnPlayCancelEvent() => OnPlayCancelEvent?.Invoke();
        public void TriggerOnPlayErrorEvent() => OnPlayErrorEvent?.Invoke();
        public void TriggerOnPlayPurchaseEvent() => OnPlayPurchaseEvent?.Invoke();

        public void TriggerOnDownloadExpansion(SoExpansionDetails val) => OnDownloadExpansion?.Invoke(val);

        public void TriggerOnSelectedABiome(int val) => OnSelectedExpansion?.Invoke(val);
        public void TriggerDeleteExpansion(int val) => OnDeleteExpansion?.Invoke(val);

        public void TriggerOnResetStore() => OnResetStore?.Invoke();
        public void TriggerOnSelectedTrack(int val) => OnSelectedTrack?.Invoke(val);
    }
}