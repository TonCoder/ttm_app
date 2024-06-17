using System;
using System.Collections.Generic;
using _MAIN_APP.Scripts.Enums;
using Unity.Collections;
using UnityEngine;

namespace _MAIN_APP.Scripts.Models
{
    [Serializable]
    public class UIDisplayData
    {
        [field: SerializeField] public int ID { get; internal set; }
        [field: SerializeField] public float Price { get; internal set; }
        [field: SerializeField] public string Title { get; internal set; }
        [field: SerializeField] public Sprite ItemImage { get; internal set; }
        [field: SerializeField] public string Artist { get; internal set; }
        [field: SerializeField] public int Qty { get; internal set; }

        [field: SerializeField] public List<ECategories> Tags { get; internal set; } = new List<ECategories>();

        public UIDisplayData()
        {
        }
    }
}