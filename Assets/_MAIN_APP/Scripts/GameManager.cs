using System;
using System.Collections.Generic;
using _MAIN_APP.Scripts.Brokers;
using _MAIN_APP.Scripts.Interfaces;
using _MAIN_APP.Scripts.ScriptableObjects;
using CreativeVeinStudio.Simple_Dialogue_System.Attributes;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.Events;
using UnityEngine.Serialization;

namespace _MAIN_APP.Scripts
{
    public class GameManager : MonoBehaviour
    {
        public static GameManager Instance;
        [SerializeField] private SoBrokerUiActions uiBroker;
        [SerializeField] private bool registered;

        [SerializeField] private AssetReference uiSfxReference;
        [SerializeField] private AssetReference availableExpansionsReference;
        [SerializeField, ExposeSo] internal SoExpansionsList ownedExpansionses;

        [SerializeField, Space(10)] private List<SoExpansionDetails> mountedExpansion = new List<SoExpansionDetails>();
        [SerializeField] private UnityEvent onInitiated;

        public SoExpansionsList AvailableExpansions { get; private set; }
        public List<SoExpansionDetails> MountedExpansion => mountedExpansion;
        public SoExpansionDetails ActiveExpansion { get; set; }
        public ITrackActions ActiveTrack { get; set; }

        private void Awake()
        {
            Singleton();
            uiBroker.OnDownloadExpansion += OnDownloadedExpansion;
        }

        private void Start()
        {
            if (AddressableManager.WasAssetDownloaded(uiSfxReference))
            {
                AddressableManager.Instance.CreateInstance(uiSfxReference);
            }

            AddressableManager.Instance.LoadAsync<SoExpansionsList>(availableExpansionsReference,
                (obj =>
                {
                    AvailableExpansions = obj;
                    onInitiated?.Invoke();
                }));
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