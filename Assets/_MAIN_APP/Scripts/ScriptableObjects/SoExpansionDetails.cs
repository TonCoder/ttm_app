using System;
using System.Collections.Generic;
using System.Linq;
using _MAIN_APP.Scripts.Enums;
using _MAIN_APP.Scripts.Models;
using JetBrains.Annotations;
using UnityEngine;

namespace _MAIN_APP.Scripts.ScriptableObjects
{
    [CreateAssetMenu(fileName = "_ExpansionDetails", menuName = "TTM/ExpansionDetails")]
    public class SoExpansionDetails : ScriptableObject
    {
        [SerializeField] private UIDisplayData details = new UIDisplayData();

        [SerializeField] [ItemCanBeNull] internal List<SoAudioTrackDetails> audioScenes =
            new List<SoAudioTrackDetails>();

        [field: SerializeField] public bool IsFree { get; private set; }
        [field: SerializeField] public bool IsActive { get; internal set; }

        public UIDisplayData Details => details;

        public IEnumerable<ECategories> GetSceneTagList =>
            audioScenes.SelectMany(x => x?.details.Tags).Distinct();

        public bool GetAudioSceneById(int id, out SoAudioTrackDetails audioTrack)
        {
            audioTrack = null;
            var exist = audioScenes.FirstOrDefault(x => x.details.ID == id);
            if (!exist) return false;
            audioTrack = exist;
            return true;
        }

        private void OnValidate()
        {
            details.Qty = audioScenes.Count;
        }
    }
}