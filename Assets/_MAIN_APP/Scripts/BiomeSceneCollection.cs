using System;
using System.Collections.Generic;
using _MAIN_APP.Scripts.Abstracts;
using _MAIN_APP.Scripts.Models;
using _MAIN_APP.Scripts.ScriptableObjects;
using EasyButtons;
using UnityEngine;

namespace _MAIN_APP.Scripts
{
    public class BiomeSceneCollection : MonoBehaviour
    {
        [SerializeField] private List<BiomeListDetails> collectionScenes = new List<BiomeListDetails>();

        public List<BiomeListDetails> Collection => collectionScenes;

        // private void Awake()
        // {
        //     if (collectionScenes.Count <= 0)
        //         GetChildScenes();
        // }

        // [Button]
        // private void GetChildScenes()
        // {
        //     if (transform.childCount <= 0) return;
        //     collectionScenes.Clear();
        //     SoAudioSceneDetails[] lst = transform.GetComponentsInChildren<SoAudioSceneDetails>();
        //     foreach (var sb in lst)
        //     {
        //         collectionScenes.Add(new BiomeListDetails()
        //         {
        //             title = sb.audioScene.details.Title,
        //             soundbankController = sb,
        //             go = sb.gameObject
        //         });
        //     }
        // }
    }
}