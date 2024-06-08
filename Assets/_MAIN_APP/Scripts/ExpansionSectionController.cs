using System;
using System.Collections.Generic;
using _MAIN_APP.Scripts.Brokers;
using _MAIN_APP.Scripts.Enums;
using _MAIN_APP.Scripts.ScriptableObjects;
using CreativeVeinStudio.Simple_Dialogue_System.Attributes;
using TMPro;
using UnityEngine;
using UnityEngine.Events;

namespace _MAIN_APP.Scripts
{
    public class ExpansionSectionController : MonoBehaviour
    {
        [FieldTitle("Sections Setupup")] [SerializeField]
        private SoBrokerUiActions uiBroker;

        [SerializeField] private Transform uiListContainer;

        [SerializeField] private Transform noExpansionNotice;

        [SerializeField] private TMP_Dropdown dropdownFilter;
        [SerializeField] private GameObject uiItemPrefab;
        [SerializeField, Space(10)] private UnityEvent onBiomeSelected;
        [SerializeField, Space(10)] private List<UiItemController> displayingItem = new List<UiItemController>();

        private int _activeFilterIndex;
        private GameManager _manager;

        private void Start()
        {
            _manager = GameManager.Instance;
            uiBroker.OnDownloadExpansion += DownloadedNewBiome;
            Init();
        }

        private void OnDisable()
        {
            uiBroker.OnDownloadExpansion -= DownloadedNewBiome;
        }

        private void DownloadedNewBiome(SoExpansionDetails soExpansionDetails)
        {
            dropdownFilter.options.Clear();
            Init();
        }

        private void Init()
        {
            SetupDropdownList();
            // create the item to the list
            foreach (var biome in _manager.ownedExpansionses.Expansions)
            {
                if (biome)
                    CreateUiButton(biome);
            }

            noExpansionNotice.gameObject.SetActive(_manager.ownedExpansionses.Expansions.Count <= 0);
        }

        private void SetupDropdownList()
        {
            // setup filter list
            dropdownFilter.onValueChanged.AddListener(OnFilterChange);

            foreach (var categories in _manager.ownedExpansionses.GetTags)
            {
                // setup dropdown options based on the filter
                dropdownFilter.options.Add(new TMP_Dropdown.OptionData(categories.ToString()));
            }

            dropdownFilter.options.Sort((a, b) => string.Compare(a.text, b.text, StringComparison.Ordinal));
            dropdownFilter.options.Insert(0, new TMP_Dropdown.OptionData("Show All"));

            dropdownFilter.captionText.text = "Show All";
            dropdownFilter.value = _activeFilterIndex;
        }

        private void CreateUiButton(in SoExpansionDetails expansion)
        {
            var item = Instantiate(uiItemPrefab, uiListContainer).GetComponent<UiItemController>();
            item.SetDisplayData(expansion.Details, OnBiomeClicked);
            displayingItem.Add(item);
        }

        private void OnBiomeClicked(int id)
        {
            uiBroker.TriggerOnSelectedABiome(id);
            onBiomeSelected?.Invoke();
        }

        private void SetExistingUIObject(int index)
        {
            displayingItem[index].SetDisplayData(_manager.ownedExpansionses.Expansions[index].Details,
                uiBroker.TriggerOnSelectedABiome);
            displayingItem[index].gameObject.SetActive(true);
        }

        private void OnFilterChange(int val)
        {
            if (val == _activeFilterIndex) return;
            _activeFilterIndex = val;
            if (val == 0) // show all
            {
                for (var i = 0; i < _manager.ownedExpansionses.Expansions.Count; i++)
                {
                    SetExistingUIObject(i);
                }

                return;
            }

            displayingItem.ForEach(x => x.gameObject.SetActive(false));
            var category = Enum.Parse<ECategories>(dropdownFilter.options[val].text);
            for (var i = 0; i < _manager.ownedExpansionses.Expansions.Count; i++)
            {
                if (!_manager.ownedExpansionses.Expansions[i].Details.Tags.Contains(category)) continue;
                displayingItem[i].gameObject.SetActive(true);
            }
        }
    }
}