using System;
using _MAIN_APP.Scripts.Interfaces;
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

        private ITrackActions _soundBank => GameManager.Instance?.ActiveTrack;

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