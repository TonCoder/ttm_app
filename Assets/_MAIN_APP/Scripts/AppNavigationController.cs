using System;
using System.Collections.Generic;
using _MAIN_APP.Scripts.Models;
using CreativeVeinStudio.Simple_Dialogue_System.Attributes;
using UnityEngine;

namespace _MAIN_APP.Scripts
{
    public class AppNavigationController : MonoBehaviour
    {
        [FieldTitle("Sections Show/Hide setup")] [SerializeField]
        private PageSections[] sectionsList;

        private Dictionary<string, PageSections> _sectionsMap = new Dictionary<string, PageSections>(5);

        private void OnValidate()
        {
            if (sectionsList != null)
                foreach (var pair in sectionsList)
                {
                    pair.sectionName = pair.eSection.ToString();
                }
        }

        private void OnEnable()
        {
            if (_sectionsMap.Count == sectionsList.Length) return;
            _sectionsMap.Clear();
            for (int i = 0; i < sectionsList.Length; i++)
            {
                _sectionsMap.Add(sectionsList[i].eSection.ToString(), sectionsList[i]);
            }
        }

        public void ShowSelected(string pageSection)
        {
            if (_sectionsMap.ContainsKey(pageSection))
                _sectionsMap[pageSection].show?.DORestart();
            else
            {
                Debug.LogWarning("The key provided does not exist in the Sections List");
                Debug.LogWarning($"- key provided {pageSection}");
            }
        }

        public void HideSelected(string pageSection)
        {
            if (_sectionsMap.ContainsKey(pageSection))
                _sectionsMap[pageSection].hide?.DORestart();
            else
            {
                Debug.LogWarning("The key provided does not exist in the Sections List");
                Debug.LogWarning($"- key provided {pageSection}");
            }
        }

        public void HideAllExceptSelected(string pageSection)
        {
            if (!_sectionsMap.ContainsKey(pageSection))
            {
                Debug.LogWarning("The key provided does not exist in the Sections List");
                Debug.LogWarning($"- key provided {pageSection}");
                return;
            }

            foreach (var pair in _sectionsMap)
            {
                if (pair.Key != pageSection.ToString())
                {
                    pair.Value.hide?.DORestart();
                }
            }

            _sectionsMap[pageSection.ToString()].show?.DORestart();
        }

        [Serializable]
        public enum ESection
        {
            Menu = 0,
            Home = 1,
            Tracks = 2,
            Shop = 3,
            AudioPlayer = 5,
            UserProfile = 4,
            ExpansionPreview = 6,
        }
    }
}