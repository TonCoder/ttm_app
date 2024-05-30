using UnityEngine;

namespace _MAIN_APP.Scripts.Interfaces
{
    public interface IAudioSceneActions
    {
        GameObject Go { get; }
        void Play();
        bool IsStillPlaying();
        void Unload();
        void LoadAndPlay();
    }
}