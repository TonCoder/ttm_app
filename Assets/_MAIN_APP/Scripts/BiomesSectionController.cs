using System;
using System.Collections.Generic;
using _MAIN_APP.Scripts.Brokers;
using _MAIN_APP.Scripts.Enums;
using _MAIN_APP.Scripts.Models;
using _MAIN_APP.Scripts.ScriptableObjects;
using CreativeVeinStudio.Simple_Dialogue_System.Attributes;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Serialization;

namespace _MAIN_APP.Scripts
{
    public class BiomesSectionController : MonoBehaviour
    {
        [Header("Sections Setup")] [SerializeField]
        private Transform uiListContainer;

        [SerializeField] private TMP_Dropdown dropdownFilter;
        [SerializeField] private GameObject uiItemPrefab;
        [SerializeField, Space(10)] private UnityEvent OnBiomeSelected;
        [SerializeField, Space(10)] private List<UiItemController> displayingItem = new List<UiItemController>();

        private int _activeFilterIndex = 0;
        private GameManager _manager;

        private void Start()
        {
            _manager = GameManager.Instance;
            BrokerUiActions.OnDownloadNewBiome += DownloadedNewBiome;
            Init();
        }

        private void OnDisable()
        {
            BrokerUiActions.OnDownloadNewBiome -= DownloadedNewBiome;
        }

        private void DownloadedNewBiome(SoBiomeDetails soBiomeDetails)
        {
            soBiomeDetails.IsActive = true;
            dropdownFilter.options.Clear();
            _manager.ownedBiomes.AddBiome(soBiomeDetails);
            Init();
        }

        private void Init()
        {
            SetupDropdownList();
            // create the item to the list
            foreach (var biome in _manager.ownedBiomes.Biomes)
            {
                if (biome)
                    CreateUiButton(biome);
            }
        }

        private void SetupDropdownList()
        {
            // setup filter list
            dropdownFilter.options.Add(new TMP_Dropdown.OptionData("Show All"));
            dropdownFilter.onValueChanged.AddListener(OnFilterChange);

            foreach (var tag in _manager.ownedBiomes.GetBiomeTagList)
            {
                // setup dropdown options based on the filter
                dropdownFilter.options.Add(new TMP_Dropdown.OptionData(tag.ToString()));
            }

            dropdownFilter.value = _activeFilterIndex;
        }

        private void CreateUiButton(in SoBiomeDetails biome)
        {
            var item = Instantiate(uiItemPrefab, uiListContainer).GetComponent<UiItemController>();
            item.SetDisplayData(biome.Details, OnBiomeClicked);
            displayingItem.Add(item);
        }

        private void OnBiomeClicked(int id)
        {
            BrokerUiActions.TriggerOnSelectedABiome(id);
            OnBiomeSelected?.Invoke();
        }

        private void SetExistingUIObject(int index)
        {
            displayingItem[index].SetDisplayData(_manager.ownedBiomes.Biomes[index].Details,
                BrokerUiActions.TriggerOnSelectedABiome);
            displayingItem[index].gameObject.SetActive(true);
        }

        private void OnFilterChange(int val)
        {
            if (val == _activeFilterIndex) return;
            _activeFilterIndex = val;
            if (val == 0) // show all
            {
                for (var i = 0; i < _manager.ownedBiomes.Biomes.Count; i++)
                {
                    SetExistingUIObject(i);
                }

                return;
            }

            displayingItem.ForEach(x => x.gameObject.SetActive(false));
            for (var i = 0; i < _manager.ownedBiomes.Biomes.Count; i++)
            {
                var t = Enum.Parse<ECategories>(dropdownFilter.options[val].text);
                if (!_manager.ownedBiomes.Biomes[i].Details.Tags.Contains(t)) continue;
                displayingItem[i].gameObject.SetActive(true);
            }
        }
    }
}