using System;
using _MAIN_APP.Scripts.Interfaces;
using _MAIN_APP.Scripts.ScriptableObjects;
using CreativeVeinStudio.Simple_Dialogue_System.Attributes;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace _MAIN_APP.Scripts
{
    public class UiAudioController : MonoBehaviour
    {
        [FieldTitle("Track info setup")] [SerializeField]
        private Image trackImage;

        [SerializeField] private TextMeshProUGUI trackName;

        [FieldTitle("Wwise Event Setup")] public AK.Wwise.Event aPauseEvent;
        public AK.Wwise.Event aStopEvent;
        public AK.Wwise.Event aResumeEvent;

        [FieldTitle("Button Events")] public UnityEvent onPlay;
        public UnityEvent onStop;
        public UnityEvent onPause;
        public UnityEvent onResume;

        private ITrackActions _soundBank => GameManager.Instance?.ActiveTrack;

        public void SetTrackInfo(SoAudioTrackDetails track)
        {
            trackImage.sprite = track.details.ItemImage;
            trackName.text = track.details.Title;
        }

        public void Play()
        {
            onPlay?.Invoke();
        }

        public void PauseAudio()
        {
            aPauseEvent.Post(_soundBank.Go);
            onPause?.Invoke();
        }

        public void StopAudio()
        {
            aStopEvent.Post(_soundBank.Go);
            onStop?.Invoke();
        }

        public void ResumeAudio()
        {
            aResumeEvent.Post(_soundBank.Go);
            onResume?.Invoke();
        }
    }
}