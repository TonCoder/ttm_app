using System;
using _MAIN_APP.Scripts.Brokers;
using UnityEngine;
using UnityEngine.Events;

namespace _MAIN_APP.Scripts
{
    public class UiAudioController : MonoBehaviour
    {
        public AK.Wwise.Event aPauseEvent;
        public AK.Wwise.Event aStopEvent;
        public AK.Wwise.Event aResumeEvent;

        public UnityEvent onPlay;
        public UnityEvent onStop;
        public UnityEvent onPause;
        public UnityEvent onResume;


        private GameObject _soundBank => GameManager.Instance?._activeScene?._audioController?.Go;


        public void Play()
        {
            onPlay?.Invoke();
        }

        public void PauseAudio()
        {
            aPauseEvent.Post(_soundBank);
            onPause?.Invoke();
        }

        public void StopAudio()
        {
            aStopEvent.Post(_soundBank);
            onStop?.Invoke();
        }

        public void ResumeAudio()
        {
            aResumeEvent.Post(_soundBank);
            onResume?.Invoke();
        }
    }
}