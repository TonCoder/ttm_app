using System;
using DG.Tweening;
using UnityEngine.Serialization;

namespace _MAIN_APP.Scripts.Models
{
    [Serializable]
    public class PageSections
    {
        public string sectionName;
        [FormerlySerializedAs("section")] public AppNavigationController.ESection eSection;
        public DOTweenAnimation show;
        public DOTweenAnimation hide;
    }
}