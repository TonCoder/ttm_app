using System;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace _MAIN_APP.Scripts.Interfaces
{
    public interface ITrackActions
    {
        GameObject Go { get; }
        int TrackID { get; set; }
        int InstanceID { get; }
        void Play();
        void Stop(Action onDone = null);
        void PlayTrack();

        bool IsStillPlaying();

        void UnloadBank(Action onDone = null);
        void Load(Action doneLoading = null);
    }
}