using System;
using System.Collections.Generic;
using System.Linq;
using _MAIN_APP.Scripts.Enums;
using _MAIN_APP.Scripts.Models;
using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace _MAIN_APP.Scripts.ScriptableObjects
{
    [CreateAssetMenu(fileName = "Expansion_", menuName = "TTM/ExpansionDetails")]
    public class SoExpansionDetails : ScriptableObject
    {
        [SerializeField] private UIDisplayData details = new UIDisplayData();

        [SerializeField] [ItemCanBeNull] internal List<SoAudioTrackDetails> audioTracks =
            new List<SoAudioTrackDetails>();

        [field: SerializeField] public AssetReference ExpansionSamplerReference { get; private set; }
        [field: SerializeField] public AssetReference ExpansionBankReference { get; private set; }


        public UIDisplayData Details
        {
            get
            {
                details.Qty = audioTracks.Count;
                return details;
            }
            internal set => details = value;
        }

        public List<ECategories> GetTrackTags =>
            audioTracks.SelectMany(x => x?.details.Tags).Distinct().ToList();

        public bool GetTrackSceneById(int id, out SoAudioTrackDetails audioTrack)
        {
            audioTrack = null;
            var exist = audioTracks.FirstOrDefault(x => x.details.ID == id);
            if (!exist) return false;
            audioTrack = exist;
            return true;
        }
    }
}