using System;
using System.Collections.Generic;
using System.Linq;
using _MAIN_APP.Scripts.Brokers;
using _MAIN_APP.Scripts.Interfaces;
using _MAIN_APP.Scripts.ScriptableObjects;
using CreativeVeinStudio.Simple_Dialogue_System.Attributes;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace _MAIN_APP.Scripts
{
    public class GameManager : MonoBehaviour
    {
        public static GameManager Instance;
        [SerializeField] private SoBrokerUiActions uiBroker;
        [SerializeField] private bool registered;

        [SerializeField, ExposeSo] internal SoExpansionsList ownedExpansionses;
        [SerializeField, ExposeSo] internal SoExpansionsList availableExpansionses;
        [SerializeField, Space(10)] private List<SoExpansionDetails> mountedExpansion = new List<SoExpansionDetails>();
        public List<SoExpansionDetails> MountedExpansion => mountedExpansion;

        public SoExpansionDetails ActiveExpansion { get; set; }
        public ITrackActions ActiveTrack { get; set; }

        // [Serializable]
        // public struct MountedExpansionInfo
        // {
        //     private bool isActive;
        //     SoExpansionDetails ActiveExpansion
        // }

        private void Awake()
        {
            Singleton();
            uiBroker.OnDownloadExpansion += OnDownloadedExpansion;
        }

        private void OnDisable()
        {
            uiBroker.OnDownloadExpansion -= OnDownloadedExpansion;
        }

        private void OnDownloadedExpansion(SoExpansionDetails soExpansionDetails)
        {
            soExpansionDetails.IsActive = true;
            ownedExpansionses.AddExpansion(soExpansionDetails);
        }

        private void Singleton()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(this.gameObject);
            }
            else
            {
                Instance = this;
                DontDestroyOnLoad(this.gameObject);
            }
        }
    }
}