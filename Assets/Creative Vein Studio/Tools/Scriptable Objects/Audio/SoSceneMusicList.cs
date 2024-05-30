using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

namespace MAIN_PROJECT._Scripts.Tools.Scriptable_Objects
{
    [CreateAssetMenu(fileName = "Scenes Music List", menuName = "CVeinStudio/Game System/Audio/ScenesMusicList")]
    public class SoSceneMusicList : ScriptableObject
    {
        [SerializeField] internal List<MusicInfo> collection = new List<MusicInfo>();
    }

    [System.Serializable]
    public class MusicInfo
    {
        [SerializeField] internal string _sceneName;
        [SerializeField] internal float _volume;
        [SerializeField] internal AudioClip[] _audioClip;
    }
}