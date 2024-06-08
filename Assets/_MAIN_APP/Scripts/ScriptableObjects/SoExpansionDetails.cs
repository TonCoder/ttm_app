using System;
using System.Collections.Generic;
using System.Linq;
using _MAIN_APP.Scripts.Enums;
using _MAIN_APP.Scripts.Models;
using JetBrains.Annotations;
using UnityEngine;

namespace _MAIN_APP.Scripts.ScriptableObjects
{
    [CreateAssetMenu(fileName = "Expansion_", menuName = "TTM/ExpansionDetails")]
    public class SoExpansionDetails : ScriptableObject
    {
        [SerializeField] private UIDisplayData details = new UIDisplayData();
        [SerializeField] private AK.Wwise.Bank expansionBank = new AK.Wwise.Bank();

        [SerializeField] [ItemCanBeNull] internal List<SoAudioTrackDetails> audioTracks =
            new List<SoAudioTrackDetails>();

        public AK.Wwise.Bank ExpansionBank => expansionBank;
        [field: SerializeField] public bool IsFree { get; private set; }
        [field: SerializeField] public bool IsActive { get; internal set; }

        public UIDisplayData Details => details;

        public IEnumerable<ECategories> GetTrackTags =>
            audioTracks.SelectMany(x => x?.details.Tags).Distinct();

        public bool GetTrackSceneById(int id, out SoAudioTrackDetails audioTrack)
        {
            audioTrack = null;
            var exist = audioTracks.FirstOrDefault(x => x.details.ID == id);
            if (!exist) return false;
            audioTrack = exist;
            return true;
        }

        private void OnValidate()
        {
            details.Qty = audioTracks.Count;
        }
    }
}