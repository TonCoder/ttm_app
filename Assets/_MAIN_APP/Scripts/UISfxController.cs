using _MAIN_APP.Scripts.Brokers;
using CreativeVeinStudio.Simple_Dialogue_System.Attributes;
using UnityEditor;
using UnityEngine;

namespace _MAIN_APP.Scripts
{
    public class UISfxController : MonoBehaviour
    {
        public SoBrokerUiActions UiBroker;

        [FieldTitle("Setup SFXs")] public AkGameObj thisGo;
        public AK.Wwise.Event playLogoEvent;
        public AK.Wwise.Event clickEvent;
        public AK.Wwise.Event cancelEvent;
        public AK.Wwise.Event confirmEvent;
        public AK.Wwise.Event errorEvent;
        public AK.Wwise.Event purchaseEvent;

        private void Start()
        {
            thisGo ??= this.gameObject.GetComponent<AkGameObj>();
            GetBrokerAndRegister();
        }

        private void GetBrokerAndRegister()
        {
            if (!GameManager.Instance?.UiBroker) return;
            UiBroker = GameManager.Instance.UiBroker;

            UiBroker.OnPlayClickEvent += PlayClick;
            UiBroker.OnPlayConfirmEvent += PlayConfirm;
            UiBroker.OnPlayCancelEvent += PlayCancel;
            UiBroker.OnPlayErrorEvent += PlayError;
            UiBroker.OnPlayPurchaseEvent += PlayPurchase;
            UiBroker.OnPlayLogoEvent += PlayLogo;
        }

        public void PlayLogo()
        {
            playLogoEvent.Post(this.gameObject);
        }

        public void PlayCancel()
        {
            cancelEvent.Post(this.gameObject);
        }

        public void PlayClick()
        {
            clickEvent.Post(this.gameObject);
        }

        public void PlayConfirm()
        {
            confirmEvent.Post(this.gameObject);
        }

        public void PlayError()
        {
            errorEvent.Post(this.gameObject);
        }

        public void PlayPurchase()
        {
            purchaseEvent.Post(this.gameObject);
        }
    }
}