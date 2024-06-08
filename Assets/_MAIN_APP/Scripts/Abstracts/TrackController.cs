using System;
using _MAIN_APP.Scripts.Brokers;
using _MAIN_APP.Scripts.Enums;
using _MAIN_APP.Scripts.Interfaces;
using _MAIN_APP.Scripts.Models;
using UnityEngine;
using UnityEngine.Assertions;

namespace _MAIN_APP.Scripts.Abstracts
{
    [Serializable, RequireComponent(typeof(AkBank), typeof(AkEvent), typeof(AkGameObj))]
    public class TrackController : MonoBehaviour, ITrackActions
    {
        [SerializeField] internal SoundBankSetup setup;

        private uint[] _playingIds = new uint[50];
        private EAudioBankStatus _bankStatus;
        public GameObject Go => this.gameObject;
        public int TrackID { get; set; }
        public int InstanceID => this.gameObject.GetInstanceID();

        private void Awake()
        {
            setup.Bank ??= GetComponent<AkBank>();
            setup.PlayEvent ??= GetComponent<AkEvent>();
            setup.Go ??= GetComponent<AkGameObj>();

            Assert.IsNotNull(setup.Bank);
            Assert.IsNotNull(setup.PlayEvent);
            Assert.IsNotNull(setup.Go);

            // Add event to the button to set self as Active biome
            // setup.Button.onClick.AddListener(SetBiome);
        }

        public void Load(Action doneLoading = null)
        {
            if (_bankStatus != EAudioBankStatus.NotLoaded) return;
            // setup.Bank.data.Load(setup.DecodeBank, setup.SaveDecodedBank);
            _bankStatus = EAudioBankStatus.Loaded;
            doneLoading?.Invoke();
        }

        public void UnloadBank(Action doneUnLoading = null)
        {
            setup.Bank.UnloadBank(this.gameObject);
            _bankStatus = EAudioBankStatus.NotLoaded;
            // AK.Wwise.Unity.WwiseAddressables.AkAddressableBankManager.Instance.
            doneUnLoading?.Invoke();
        }

        public bool IsStillPlaying()
        {
            var e = AkSoundEngine.GetIDFromString(setup.PlayEvent.data.ObjectReference.DisplayName);
            uint count = (uint)_playingIds.Length;
            AkSoundEngine.GetPlayingIDsFromGameObject((uint)gameObject.GetInstanceID(), ref count, _playingIds);

            for (int i = 0; i < count; i++)
            {
                uint playingId = _playingIds[i];
                uint eventId = AkSoundEngine.GetEventIDFromPlayingID(playingId);

                if (eventId == e)
                    return true;
            }

            return false;
        }

        public void PlayTrack()
        {
            setup.PlayEvent.data.Stop(this.gameObject);
            setup.PlayEvent.data.Post(this.gameObject, (uint)AkCallbackType.AK_EndOfEvent,
                MusicDonePlaying);
        }

        public void Play()
        {
            switch (_bankStatus)
            {
                case EAudioBankStatus.NotLoaded:
                    Debug.Log("Unable to play SB since it's not loaded yet");
                    break;
                case EAudioBankStatus.Loaded:
                    _bankStatus = EAudioBankStatus.Playing;
                    setup.PlayEvent.data.Post(this.gameObject, (uint)AkCallbackType.AK_EndOfEvent,
                        MusicDonePlaying);
                    break;
                case EAudioBankStatus.Loading:
                    Debug.Log("Sound Track is still loading");
                    break;
                case EAudioBankStatus.Playing:
                    Debug.Log("Already playing Sound Track");
                    break;
                default:
                    Debug.Log("Not a valid Sound Track status");
                    break;
            }
        }

        public void Stop(Action stopped)
        {
            setup.PlayEvent.data.Stop(this.gameObject);
        }

        private void MusicDonePlaying(object inCookie, AkCallbackType inType, AkCallbackInfo inInfo)
        {
            if (inType == AkCallbackType.AK_EndOfEvent)
            {
                Debug.Log("Done playing!");
                _bankStatus = EAudioBankStatus.Loaded;
            }
        }
    }
}