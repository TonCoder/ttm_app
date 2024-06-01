using System;
using System.Collections.Generic;
using System.Linq;
using _MAIN_APP.Scripts.Enums;
using EasyButtons;

#if UNITY_EDITOR
using UnityEditor;
#endif

using UnityEngine;
using UnityEngine.Serialization;

namespace _MAIN_APP.Scripts.ScriptableObjects
{
    [CreateAssetMenu(fileName = "Expansions_", menuName = "TTM/List/Expansions")]
    public class SoExpansionsList : ScriptableObject
    {
        [SerializeField] private List<SoExpansionDetails> expansions = new List<SoExpansionDetails>();

        public List<SoExpansionDetails> Expansions => expansions;

        private void OnValidate()
        {
            if (expansions.All(c => c != null)) return;
            Debug.LogError("There are empty values in the SoBiomeList");
            Debug.Break();
        }

#if UNITY_EDITOR
        [Tooltip("Test purpose only; gets all biomes at given path")]
        public string biomePath = "Assets/_MAIN_APP/content/Objects/Biomes";

        [Button("Get Biomes")]
        public void GetBiomeScriptables()
        {
            expansions.Clear();
            string[] guids2 = AssetDatabase.FindAssets("_Expansions", new[] { biomePath });

            foreach (var s in guids2)
            {
                expansions.Add(AssetDatabase.LoadAssetAtPath<SoExpansionDetails>(AssetDatabase.GUIDToAssetPath(s)));
            }
        }
#endif

        public List<ECategories> GetTags => expansions?.SelectMany(x => x.Details.Tags).Distinct().ToList();

        public void AddExpansion(SoExpansionDetails expansion)
        {
            if (!expansions.Contains(expansion))
            {
                expansions.Add(expansion);
                return;
            }

            Debug.Log("Already added");
        }

        public void RemoveExpansion(SoExpansionDetails expansion)
        {
            if (expansions.Remove(expansion))
            {
                Debug.Log("Removed");
                return;
            }

            Debug.Log("Biome doesnt exists in owned list");
        }

        public bool GetExpansionById(int id, out SoExpansionDetails expansion)
        {
            expansion = null;
            var exist = expansions.FirstOrDefault(x => x.Details.ID == id);
            if (!exist) return false;
            expansion = exist;
            return true;
        }

        public bool ContainsItem(int biomeID)
        {
            return expansions.Any(x => x.Details.ID == biomeID);
        }
    }
}