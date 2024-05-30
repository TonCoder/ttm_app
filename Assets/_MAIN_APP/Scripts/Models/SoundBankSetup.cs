using System;
using UnityEngine;
using UnityEngine.UI;

namespace _MAIN_APP.Scripts.Models
{
    [Serializable]
    public class SoundBankSetup
    {
        [field: SerializeField] public AkGameObj Go { get; internal set; }
        [field: SerializeField] public AkBank Bank { get; internal set; }
        [field: SerializeField] public AkEvent PlayEvent { get; internal set; }
        [field: SerializeField] public bool DecodeBank { get; internal set; }
        [field: SerializeField] public bool SaveDecodedBank { get; internal set; }
    }
}