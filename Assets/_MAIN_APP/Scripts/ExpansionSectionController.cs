using System;
using System.Collections.Generic;
using System.Linq;
using _MAIN_APP.Scripts.Brokers;
using _MAIN_APP.Scripts.Enums;
using _MAIN_APP.Scripts.ScriptableObjects;
using TMPro;
using UnityEngine;
using UnityEngine.Events;

namespace _MAIN_APP.Scripts
{
    public class ExpansionSectionController : MonoBehaviour
    {
        [Header("Sections Setup")] [SerializeField]
        private Transform uiListContainer;

        [SerializeField] private Transform noExpansionNotice;

        [SerializeField] private TMP_Dropdown dropdownFilter;
        [SerializeField] private GameObject uiItemPrefab;
        [SerializeField, Space(10)] private UnityEvent OnBiomeSelected;
        [SerializeField, Space(10)] private List<UiItemController> displayingItem = new List<UiItemController>();

        private int _activeFilterIndex = 0;
        private GameManager _manager;

        private void Start()
        {
            _manager = GameManager.Instance;
            BrokerUiActions.OnDownloadExpansion += DownloadedNewBiome;
            Init();
        }

        private void OnDisable()
        {
            BrokerUiActions.OnDownloadExpansion -= DownloadedNewBiome;
        }

        private void DownloadedNewBiome(SoExpansionDetails soExpansionDetails)
        {
            soExpansionDetails.IsActive = true;
            dropdownFilter.options.Clear();
            _manager.ownedExpansionses.AddExpansion(soExpansionDetails);
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

            foreach (var tag in _manager.ownedExpansionses.GetTags)
            {
                // setup dropdown options based on the filter
                dropdownFilter.options.Add(new TMP_Dropdown.OptionData(tag.ToString()));
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
            BrokerUiActions.TriggerOnSelectedABiome(id);
            OnBiomeSelected?.Invoke();
        }

        private void SetExistingUIObject(int index)
        {
            displayingItem[index].SetDisplayData(_manager.ownedExpansionses.Expansions[index].Details,
                BrokerUiActions.TriggerOnSelectedABiome);
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