using System;
using System.Collections.Generic;
using System.Linq;
using _MAIN_APP.Scripts.Brokers;
using _MAIN_APP.Scripts.Interfaces;
using _MAIN_APP.Scripts.ScriptableObjects;
using CreativeVeinStudio.Simple_Dialogue_System.Attributes;
using EasyButtons;
using HeistEscape;
using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.Events;
using UnityEngine.Serialization;

namespace _MAIN_APP.Scripts
{
    public class GameManager : MonoBehaviour
    {
        public static GameManager Instance;

        #region Exposed Vars

        [SerializeField] private SoBrokerUiActions uiBroker;
        [SerializeField] private bool registered;

        [SerializeField] private AssetReference uiSfxReference;
        [SerializeField] private AssetReference availableExpansionsReference;

        [FormerlySerializedAs("ownedExpansionses")] [SerializeField, ExposeSo]
        internal SoExpansionsList ownedExpansions;

        [SerializeField, Space(10)] private List<SoExpansionDetails> mountedExpansion = new List<SoExpansionDetails>();
        [SerializeField] private UnityEvent onInitiated;

        #endregion

        #region Props

        [field: SerializeField] public SoExpansionsList AvailableExpansionInst { get; private set; }
        [field: SerializeField] public SoExpansionDetails ActiveExpansion { get; internal set; }
        public List<SoExpansionDetails> MountedExpansion => mountedExpansion;
        public ITrackActions ActiveTrack { get; internal set; }
        public SoBrokerUiActions UiBroker => uiBroker;

        #endregion

        private void Awake()
        {
            Singleton();
            uiBroker.OnDownloadExpansion += OnDownloadedExpansion;
            uiBroker.OnDeleteExpansion += DeleteExpansionFromList;
        }

        private void Start()
        {
            // creates SFX instance
            AddressableManager.Instance.CreateInstance(uiSfxReference);
            GetUserData();
        }

        private void OnDisable()
        {
            uiBroker.OnDownloadExpansion -= OnDownloadedExpansion;
        }

        [Button]
        private void GetUserData()
        {
            // get data from Player prefs if exist
            var ownedIds = GamePrefs.GetPrefs<List<string>>();

            // load store data
            LoadStoreExpansions(ownedIds);
        }

        [Button]
        private void SaveUserData()
        {
            GamePrefs.SavePrefs(ownedExpansions.Expansions.Select(x => x.Details.ID).ToList());
        }

        private void LoadStoreExpansions([CanBeNull] List<string> ownedIds)
        {
            // creates scriptable expansion reference
            AddressableManager.Instance.LoadAsync<SoExpansionsList>(availableExpansionsReference,
                (obj =>
                {
                    AvailableExpansionInst = obj;
                    GetOwnedExpansions(AvailableExpansionInst, ownedIds);
                    onInitiated?.Invoke();
                }));
        }

        private void GetOwnedExpansions(SoExpansionsList available, List<string> ownedIds)
        {
            ownedIds.ForEach(x =>
            {
                if (available.GetExpansionById(int.Parse(x), out var expansion))
                {
                    ownedExpansions.AddExpansion(expansion);
                }
            });
        }

        private void OnDownloadedExpansion(SoExpansionDetails soExpansionDetails)
        {
            if (!soExpansionDetails) return;
            soExpansionDetails.IsActive = true;
            ownedExpansions.AddExpansion(soExpansionDetails);
        }

        private void DeleteExpansionFromList(int id)
        {
            if (ownedExpansions.GetExpansionById(id, out var expansion))
            {
                AddressableManager.Instance?.UnLoadAndDestroy(expansion.ExpansionBankReference);
                ownedExpansions?.RemoveExpansion(expansion);
                AddressableManager.Instance?.DeleteCacheAndDownload(expansion.ExpansionBankReference);
                uiBroker.TriggerOnResetStore();
            }
        }

        private void Singleton()
        {
            // If there is an instance, and it's not me, delete myself.
            if (Instance != null && Instance != this)
            {
                Destroy(this);
            }
            else
            {
                Instance = this;
                DontDestroyOnLoad(this.gameObject);
            }
        }
    }
}