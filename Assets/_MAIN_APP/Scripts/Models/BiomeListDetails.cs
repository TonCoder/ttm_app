using System;
using _MAIN_APP.Scripts.ScriptableObjects;
using UnityEngine;
using UnityEngine.Serialization;

namespace _MAIN_APP.Scripts.Models
{
    [Serializable]
    public class BiomeListDetails
    {
        public string title;
        [FormerlySerializedAs("audioSceneDetails")] public SoAudioTrackDetails audioTrackDetails;
        public GameObject go;
    }
}