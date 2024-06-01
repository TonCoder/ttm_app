using System;
using System.Collections.Generic;
using _MAIN_APP.Scripts.Brokers;
using _MAIN_APP.Scripts.Enums;
using CreativeVeinStudio.Simple_Dialogue_System.Attributes;
using TMPro;
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
        private BrokerSoundBank soundbankBroker;

        [SerializeField, Tooltip("The container in which the list of scenes will show up in")]
        private Transform uiListContainer;

        [SerializeField, Tooltip("The prefab button to use within scenes section")]
        private GameObject uiButtonPrefab;

        [SerializeField] private TMP_Dropdown dropdownFilter;

        [SerializeField] private List<DisplayListData> displayingItem = new List<DisplayListData>();

        [FieldTitle("Sections Setup")] [SerializeField]
        private UnityEvent onSceneSelected;


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
            BrokerUiActions.OnSelectedExpansion += OnBiomeSelected;
        }

        private void Start()
        {
            _manager = GameManager.Instance;
        }

        private void OnDisable()
        {
            BrokerUiActions.OnSelectedExpansion -= OnBiomeSelected;
            displayingItem.Clear();
        }

        private void SetupDropdownList()
        {
            // setup filter list
            dropdownFilter.onValueChanged.AddListener(OnFilterChange);

            foreach (var tag in _manager.activeExpansion?.GetSceneTagList!)
            {
                // setup dropdown options based on the filter
                dropdownFilter.options.Add(new TMP_Dropdown.OptionData(tag.ToString()));
            }

            dropdownFilter.options.Sort((a, b) => string.Compare(a.text, b.text, StringComparison.Ordinal));
            dropdownFilter.options.Insert(0, new TMP_Dropdown.OptionData("Show All"));

            dropdownFilter.captionText.text = "Show All";
            dropdownFilter.value = _activeFilterIndex;
        }

        private void OnBiomeSelected(int biomeSelected)
        {
            Debug.Log($"Biome selected {biomeSelected}");
            // TODO - show loading 

            if (_manager.activeExpansion && _manager.activeExpansion.Details.ID == biomeSelected) return;

            if (_manager.ownedExpansionses.GetExpansionById(biomeSelected, out _manager.activeExpansion))
            {
                // remove any previous reference, from previously active biome
                _manager.activeExpansion?.audioScenes?.ForEach(x => x.DestroyReference());

                // hide buttons if created for reuse
                displayingItem.ForEach(x => x.Go.SetActive(false));

                // create Tag list for dropdown filter
                SetupDropdownList();

                for (int i = 0; i < _manager.activeExpansion?.Details.Qty; i++)
                {
                    CreateUiButton(i);
                }


                // set Scene Biome header info
                trackHeaderUI.SetDisplayData(_manager.activeExpansion.Details, null);

                // TODO - hide loading 

                return;
            }

            Debug.Log("The selected biome does not exist");
        }

        private void CreateUiButton(int index)
        {
            if (_manager.activeExpansion.audioScenes == null || _manager.activeExpansion.audioScenes?.Count <= 0)
            {
                Debug.Log("The Biome was not loaded or something went wrong.");
                return;
            }


            if (displayingItem != null && displayingItem.Count > index)
            {
                displayingItem[index].uiItemController
                    .SetDisplayData(_manager.activeExpansion.audioScenes?[index]?.details, OnSceneSelected);
                displayingItem[index].Go.SetActive(true);
            }
            else
            {
                // instantiate object to display
                var item = Instantiate(uiButtonPrefab, uiListContainer).GetComponent<UiItemController>();
                item.SetDisplayData(_manager.activeExpansion.audioScenes?[index]?.details, OnSceneSelected);

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
            if (_manager.activeExpansion.audioScenes != null &&
                (_manager.activeTrack?.details?.ID ?? 0) == selectedId) return;

            _manager.activeTrack?.UnloadReference();
            if (_manager.activeExpansion.GetAudioSceneById(selectedId, out _manager.activeTrack))
            {
                _manager.activeTrack?.CreateOrLoadReference();
            }

            onSceneSelected?.Invoke();
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
            var category = Enum.Parse<ECategories>(dropdownFilter.options[val].text);
            for (var i = 0; i < _manager.activeExpansion.audioScenes.Count; i++)
            {
                if (!_manager.activeExpansion.audioScenes[i]!.details.Tags.Contains(category)) continue;
                displayingItem[i].Go.SetActive(true);
            }
        }
    }
}