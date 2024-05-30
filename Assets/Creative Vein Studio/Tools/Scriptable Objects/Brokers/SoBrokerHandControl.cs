using System;
using UnityEngine;

namespace _Main_App.Scripts.brokers
{
    [CreateAssetMenu(fileName = "HandBroker", menuName = "CVeinStudio/Brokers/HandBroker")]
    public class SoBrokerHandControl : ScriptableObject
    {
        public static Action<Vector3> OnTapped;
        public static Action<IPlayerHandActions> OnTryGrabBob;
        public static Action<IPlayerHandActions> OnTryGrabObject;
        public static Action OnReleaseObject;
        public static Action OnDropBob { get; set; }

        public void TriggerOnTapped(Vector3 val) => OnTapped?.Invoke(val);
        public void TriggerOnTryGrabObject(IPlayerHandActions val) => OnTryGrabObject?.Invoke(val);
        public void TriggerOnDropObject() => OnReleaseObject?.Invoke();

        public void TriggerDropBob() => OnDropBob?.Invoke();
        public void TriggerGrabBob(IPlayerHandActions playerHand) => OnTryGrabBob?.Invoke(playerHand);
    }

    public interface IPlayerHandActions
    {
    }
}