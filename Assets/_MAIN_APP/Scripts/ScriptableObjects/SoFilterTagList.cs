using System.Collections.Generic;
using UnityEngine;

namespace _MAIN_APP.Scripts.ScriptableObjects
{
    [CreateAssetMenu(fileName = "Tags", menuName = "TTM/List/Tags")]
    public class SoFilterTagList : ScriptableObject
    {
        [field: SerializeField]
        public List<string> List { get; private set; } = new()
        {
            "Forrest",
            "Combat",
            "Dungeon",
            "--Themes below--",
            "Fantasy",
            "Cyber",
            "Apocalypse",
            "Casual",
            "Therapy",
        };
    }
}