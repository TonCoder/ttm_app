using System;
using System.Collections.Generic;
using System.Linq;
using _MAIN_APP.Scripts.Brokers;
using _MAIN_APP.Scripts.Enums;
using _MAIN_APP.Scripts.Models;
using TMPro;
using UnityEngine;

namespace _MAIN_APP.Scripts
{
    public class SceneSectionController : MonoBehaviour
    {
        [Header("Biome UI Setup")] [SerializeField]
        private UiItemController sceneBiomeHeaderUI;

        [Header("Sections Setup")] [SerializeField]
        private BrokerSoundBank soundbankBroker;

        [SerializeField, Tooltip("The container in which the list of scenes will show up in")]
        private Transform uiListContainer;

        [SerializeField, Tooltip("The prefab button to use within scenes section")]
        private GameObject uiButtonPrefab;

        [SerializeField] private TMP_Dropdown dropdownFilter;

        [SerializeField] private List<DisplayListData> displayingItem = new List<DisplayListData>();

        private int _activeFilterIndex = 0;
        private GameManager _manager;

        [Serializable]
        struct DisplayListData
        {
            public GameObject Go;
            public UiItemController uiItemController;
        }

        private void Awake()
        {
            BrokerUiActions.OnSelectedABiome += OnBiomeSelected;
        }

        private void Start()
        {
            _manager = GameManager.Instance;
        }

        private void OnDisable()
        {
            BrokerUiActions.OnSelectedABiome -= OnBiomeSelected;
            displayingItem.Clear();
        }

        private void SetupDropdownList()
        {
            // setup filter list
            dropdownFilter.options.Add(new TMP_Dropdown.OptionData("Show All"));
            dropdownFilter.onValueChanged.AddListener(OnFilterChange);

            foreach (var tag in _manager._activeBiome?.GetSceneTagList!)
            {
                // setup dropdown options based on the filter
                dropdownFilter.options.Add(new TMP_Dropdown.OptionData(tag.ToString()));
            }

            dropdownFilter.captionText.text = "Show All";
            dropdownFilter.value = _activeFilterIndex;
        }

        private void OnBiomeSelected(int biomeSelected)
        {
            Debug.Log($"Biome selected {biomeSelected}");
            // TODO - show loading 

            if (_manager._activeBiome && _manager._activeBiome.Details.ID == biomeSelected) return;

            if (_manager.ownedBiomes.GetBiomeById(biomeSelected, out _manager._activeBiome))
            {
                // remove any previous reference, from previously active biome
                _manager._activeBiome?.audioScenes?.ForEach(x => x.DestroyReference());

                // hide buttons if created for reuse
                displayingItem.ForEach(x => x.Go.SetActive(false));

                // create Tag list for dropdown filter
                SetupDropdownList();

                for (int i = 0; i < _manager._activeBiome?.Details.Qty; i++)
                {
                    CreateUiButton(i);
                }


                // set Scene Biome header info
                sceneBiomeHeaderUI.SetDisplayData(_manager._activeBiome.Details, null);

                // TODO - hide loading 

                return;
            }

            Debug.Log("The selected biome does not exist");
        }

        private void CreateUiButton(int index)
        {
            if (_manager._activeBiome.audioScenes == null || _manager._activeBiome.audioScenes?.Count <= 0)
            {
                Debug.Log("The Biome was not loaded or something went wrong.");
                return;
            }


            if (displayingItem != null && displayingItem.Count > index)
            {
                displayingItem[index].uiItemController
                    .SetDisplayData(_manager._activeBiome.audioScenes?[index]?.details, OnSceneSelected);
                displayingItem[index].Go.SetActive(true);
            }
            else
            {
                // instantiate object to display
                var item = Instantiate(uiButtonPrefab, uiListContainer).GetComponent<UiItemController>();
                item.SetDisplayData(_manager._activeBiome.audioScenes?[index]?.details, OnSceneSelected);

                displayingItem.Add(new DisplayListData()
                {
                    Go = item.gameObject,
                    uiItemController = item.GetComponent<UiItemController>()
                });
            }
        }

        private void OnSceneSelected(int selectedId)
        {
            //if same scene selected then do nothing
            if (_manager._activeBiome.audioScenes != null &&
                _manager._activeScene.details.ID == selectedId) return;

            _manager._activeScene?.UnloadReference();
            if (_manager._activeBiome.GetAudioSceneById(selectedId, out _manager._activeScene))
            {
                _manager._activeScene?.CreateOrLoadReference();
            }
        }

        private void OnFilterChange(int val)
        {
            if (val == _activeFilterIndex) return;
            _activeFilterIndex = val;
            if (val == 0) // show all
            {
                displayingItem.ForEach(x => x.Go.SetActive(true));
                return;
            }

            displayingItem.ForEach(x => x.Go.SetActive(false));
            for (var i = 0; i < _manager._activeBiome.audioScenes.Count; i++)
            {
                var t = Enum.Parse<ECategories>(dropdownFilter.options[val].text);
                if (!_manager._activeBiome.audioScenes[i]!.details.Tags.Contains(t)) continue;
                displayingItem[i].Go.SetActive(true);
            }
        }
    }
}