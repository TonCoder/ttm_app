using System;
using System.Collections.Generic;
using System.Linq;
using _MAIN_APP.Scripts.Brokers;
using _MAIN_APP.Scripts.Enums;
using _MAIN_APP.Scripts.Interfaces;
using _MAIN_APP.Scripts.ScriptableObjects;
using CreativeVeinStudio.Simple_Dialogue_System.Attributes;
using TMPro;
using UltEvents;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Serialization;

namespace _MAIN_APP.Scripts
{
    public class TracksSectionController : MonoBehaviour
    {
        [FormerlySerializedAs("sceneBiomeHeaderUI")] [FieldTitle("Biome UI Setup")] [SerializeField]
        private UiItemController trackHeaderUI;

        [FieldTitle("Sections Setup")] [SerializeField]
        private SoBrokerUiActions uiBroker;

        [SerializeField, Tooltip("The container in which the list of scenes will show up in")]
        private Transform uiListContainer;

        [SerializeField, Tooltip("The prefab button to use within scenes section")]
        private GameObject uiButtonPrefab;

        [SerializeField] private TMP_Dropdown dropdownFilter;

        [SerializeField] private List<DisplayListData> displayingItem = new List<DisplayListData>();

        [FieldTitle("Sections Setup")] [SerializeField]
        private UltEvent<SoAudioTrackDetails> onTrackSelected;

        private SoExpansionDetails _currentDisplayingExpansion;
        private int _activeFilterIndex;
        private GameManager _manager;
        private List<SoExpansionDetails> _expansionToUnload = new List<SoExpansionDetails>(20);

        struct DisplayListData
        {
            public UiItemController uiItemController;
            public GameObject go;
        }

        private void Awake()
        {
            uiBroker.OnSelectedExpansion += OnSelectedExpansion;
        }

        private void Start()
        {
            _manager = GameManager.Instance;
        }

        private void OnDisable()
        {
            uiBroker.OnSelectedExpansion -= OnSelectedExpansion;
            displayingItem.Clear();
        }

        public void RemoveUnusedBanks()
        {
            _expansionToUnload =
                _manager.MountedExpansion.Where(x => x != _manager.ActiveExpansion).ToList();

            // do not keep more than One extra banks loaded
            if (_expansionToUnload.Count <= 1) return;
            _expansionToUnload?.ForEach(x =>
                {
                    AddressableManager.Instance.UnLoadAndDestroy(x.ExpansionBankReference);
                    _manager.MountedExpansion.Remove(x);
                }
            );
        }

        private void OnSelectedExpansion(int biomeSelected)
        {
#if UNITY_EDITOR
            Debug.Log($"Biome selected {biomeSelected}");
#endif
            if (_manager.ActiveExpansion && _manager.ActiveExpansion.Details.ID == biomeSelected) return;

            if (_manager.ownedExpansions.GetExpansionById(biomeSelected, out var expansion))
            {
                _currentDisplayingExpansion = expansion;
                // add expansion and load the SoundBank
                _manager.MountedExpansion.Add(expansion);

                AddressableManager.Instance.CreateInstance(expansion.ExpansionBankReference);

                // hide buttons if any for reuse
                displayingItem.ForEach(x => x.go.SetActive(false));

                // create Tag list for dropdown filter
                TtmUtilities.SetupDropdownList(ref dropdownFilter, expansion.GetTrackTags, null,
                    OnFilterChange);

                for (int i = 0; i < expansion?.audioTracks.Count; i++)
                {
                    CreateUiButton(expansion, i);
                }

                // set Scene Expansion header info
                trackHeaderUI.SetDisplayData(expansion.Details);
                return;
            }
#if UNITY_EDITOR
            Debug.Log("The selected biome does not exist");
#endif
        }

        private void OnTrackSelected(int selectedId)
        {
            _manager.ActiveExpansion = _currentDisplayingExpansion;

            if (_manager.ActiveExpansion.GetTrackSceneById(selectedId, out SoAudioTrackDetails track))
            {
                if (_manager.ActiveTrack != null && _manager.ActiveTrack.TrackID != track.details.ID)
                {
                    _manager.ActiveTrack.Stop(() =>
                        AddressableManager.Instance.UnLoadAndDestroy(_manager.ActiveTrack.InstanceID));
                }

                // get the audio track and create a ref
                _manager.ActiveTrack = AddressableManager.Instance.CreateInstance(track.AudioReference)
                    .GetComponent<ITrackActions>();

                // play new track
                _manager.ActiveTrack.TrackID = track.details.ID;
                _manager.ActiveTrack.Load();
                _manager.ActiveTrack.Play();
                onTrackSelected?.Invoke(track);
            }
        }

        private void CreateUiButton(SoExpansionDetails selectedExpansion, int index)
        {
            if (selectedExpansion.audioTracks == null ||
                selectedExpansion.audioTracks?.Count <= 0)
            {
#if UNITY_EDITOR
                Debug.Log("The Expansion was not loaded or something went wrong.");
#endif
                return;
            }

            if (displayingItem != null && displayingItem.Count > index)
            {
                displayingItem[index].uiItemController
                    .SetDisplayData(selectedExpansion.audioTracks?[index]?.details, OnTrackSelected);
                displayingItem[index].go.SetActive(true);
            }
            else
            {
                // instantiate object to display
                var trackUI = Instantiate(uiButtonPrefab, uiListContainer).GetComponent<UiItemController>();
                trackUI.SetDisplayData(selectedExpansion.audioTracks?[index]?.details, OnTrackSelected);

                displayingItem.Add(new DisplayListData()
                {
                    go = trackUI.gameObject,
                    uiItemController = trackUI.GetComponent<UiItemController>()
                });
            }
        }

        private void OnFilterChange(int val)
        {
            if (val == _activeFilterIndex) return;
            _activeFilterIndex = val;
            if (val == 0) // show all
            {
                displayingItem.ForEach(x => x.go.SetActive(true));
                return;
            }

            displayingItem.ForEach(x => x.go.SetActive(false));
            var category = Enum.Parse<ECategories>(dropdownFilter.options[val].text);
            for (var i = 0; i < _manager.ActiveExpansion.audioTracks.Count; i++)
            {
                if (!_manager.ActiveExpansion.audioTracks[i]!.details.Tags.Contains(category)) continue;
                displayingItem[i].go.SetActive(true);
            }
        }
    }
}