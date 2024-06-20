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
            Init();
        }

        public void Init()
        {
            if (!_manager?.ownedExpansions) return;

            SetupDropdownList();

            // create the item to the list
            for (var i = 0; i < _manager.ownedExpansions.Expansions.Count; i++)
            {
                var biome = _manager.ownedExpansions.Expansions[i];
                if (biome)
                {
                    if ((displayingItem.Count - 1) >= i)
                    {
                        displayingItem[i].SetDisplayData(biome.Details, OnBiomeClicked, OnDeleteBiomeClicked);
                    }
                    else
                    {
                        CreateUiButton(biome);
                    }
                }
            }

            noExpansionNotice.gameObject.SetActive(_manager.ownedExpansions.Expansions.Count <= 0);
        }

        private void SetupDropdownList()
        {
            dropdownFilter.options.Clear();
            TtmUtilities.SetupDropdownList(ref dropdownFilter, _manager.ownedExpansions.GetTags, null, OnFilterChange);
        }

        private void CreateUiButton(in SoExpansionDetails expansion)
        {
            var item = Instantiate(uiItemPrefab, uiListContainer).GetComponent<UiItemController>();
            item.SetDisplayData(expansion.Details, OnBiomeClicked, OnDeleteBiomeClicked);
            displayingItem.Add(item);
        }

        private void OnBiomeClicked(int id)
        {
            uiBroker.TriggerOnSelectedABiome(id);
            onBiomeSelected?.Invoke();
        }

        private void OnDeleteBiomeClicked(int id)
        {
            uiBroker.TriggerDeleteExpansion(id);
        }

        private void SetExistingUIObject(int index)
        {
            displayingItem[index].SetDisplayData(_manager.ownedExpansions.Expansions[index].Details,
                uiBroker.TriggerOnSelectedABiome);
            displayingItem[index].gameObject.SetActive(true);
        }

        private void OnFilterChange(int val)
        {
            if (val == _activeFilterIndex) return;
            _activeFilterIndex = val;
            if (val == 0) // show all
            {
                for (var i = 0; i < _manager.ownedExpansions.Expansions.Count; i++)
                {
                    SetExistingUIObject(i);
                }

                return;
            }

            displayingItem.ForEach(x => x.gameObject.SetActive(false));
            var category = Enum.Parse<ECategories>(dropdownFilter.options[val].text);
            for (var i = 0; i < _manager.ownedExpansions.Expansions.Count; i++)
            {
                if (!_manager.ownedExpansions.Expansions[i].Details.Tags.Contains(category)) continue;
                displayingItem[i].gameObject.SetActive(true);
            }
        }
    }
}