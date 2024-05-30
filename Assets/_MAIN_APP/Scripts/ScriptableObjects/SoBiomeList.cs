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
    [CreateAssetMenu(fileName = "Biomes_", menuName = "TTM/List/Biomes")]
    public class SoBiomeList : ScriptableObject
    {
        [SerializeField] private List<SoBiomeDetails> biomes = new List<SoBiomeDetails>();

        public List<SoBiomeDetails> Biomes => biomes;

        private void OnValidate()
        {
            if (biomes.All(c => c != null)) return;
            Debug.LogError("There are empty values in the SoBiomeList");
            Debug.Break();
        }

#if UNITY_EDITOR
        [Tooltip("Test purpose only; gets all biomes at given path")]
        public string biomePath = "Assets/_MAIN_APP/content/Objects/Biomes";

        [Button("Get Biomes")]
        public void GetBiomeScriptables()
        {
            biomes.Clear();
            string[] guids2 = AssetDatabase.FindAssets("_Biome", new[] { biomePath });

            foreach (var s in guids2)
            {
                biomes.Add(AssetDatabase.LoadAssetAtPath<SoBiomeDetails>(AssetDatabase.GUIDToAssetPath(s)));
            }
        }
#endif

        public IEnumerable<ECategories> GetBiomeTagList => biomes?.SelectMany(x => x.Details.Tags);

        public void AddBiome(SoBiomeDetails biome)
        {
            if (!biomes.Contains(biome))
            {
                biomes.Add(biome);
                return;
            }

            Debug.Log("Already added");
        }

        public void RemoveBiome(SoBiomeDetails biome)
        {
            if (biomes.Remove(biome))
            {
                Debug.Log("Removed");
                return;
            }

            Debug.Log("Biome doesnt exists in owned list");
        }

        public bool GetBiomeById(int id, out SoBiomeDetails biome)
        {
            biome = null;
            var exist = biomes.FirstOrDefault(x => x.Details.ID == id);
            if (!exist) return false;
            biome = exist;
            return true;
        }

        public bool ContainsItem(int biomeID)
        {
            return biomes.Any(x => x.Details.ID == biomeID);
        }
    }
}