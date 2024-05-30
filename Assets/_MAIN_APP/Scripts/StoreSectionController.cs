using System;
using System.Collections.Generic;
using System.Linq;
using _MAIN_APP.Scripts.Brokers;
using _MAIN_APP.Scripts.Enums;
using _MAIN_APP.Scripts.ScriptableObjects;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;

namespace _MAIN_APP.Scripts
{
    public class StoreSectionController : MonoBehaviour
    {
        [Header("Sections Setup")] [SerializeField]
        private Transform uiListContainer;

        [SerializeField] private TMP_Dropdown dropdownFilter;

        [FormerlySerializedAs("uiItemPrefab")] [SerializeField]
        private GameObject uiButtonPrefab;

        [SerializeField] private List<UiItemController> displayingItem = new List<UiItemController>();

        private int _activeFilterIndex = 0;

        private GameManager _manager;

        private void Start()
        {
            _manager = GameManager.Instance;
            dropdownFilter.options.Clear();
            Init();
        }

        private void Init()
        {
            // setup filter list
            dropdownFilter.options.Add(new TMP_Dropdown.OptionData("Show All"));
            dropdownFilter.options.Add(new TMP_Dropdown.OptionData("Owned"));
            dropdownFilter.options.Add(new TMP_Dropdown.OptionData("Free"));
            dropdownFilter.onValueChanged.AddListener(OnFilterChange);
            dropdownFilter.captionText.text = "Show All";

            foreach (var tag in _manager.availableBiomes.GetBiomeTagList)
            {
                // setup dropdown options based on the filter
                dropdownFilter.options.Add(new TMP_Dropdown.OptionData(tag.ToString()));
            }

            // create the item to the list
            foreach (var biome in _manager.availableBiomes.Biomes)
            {
                CreateUiButton(biome);
            }

            dropdownFilter.value = _activeFilterIndex;
        }

        private void CreateUiButton(in SoBiomeDetails biome)
        {
            var item = Instantiate(uiButtonPrefab, uiListContainer).GetComponent<UiItemController>();
            item.SetDisplayData(biome.IsFree,
                biome.IsActive,
                biome.Details,
                DownloadSelectedBiome);
            displayingItem.Add(item);
        }

        private void DownloadSelectedBiome(int id)
        {
            // perform download on the object with addressable
            if (_manager.availableBiomes.GetBiomeById(id, out var selectedBiome))
            {
                var scenesToCheck = selectedBiome.audioScenes?.Select(x => x?.AudioReference).ToArray();

                if (!AddressableManager.WereAssetsDownloaded(scenesToCheck))
                {
                    Debug.Log("Need to download Biome scenes");
                    //TODO SHOW PURCHASE VALIDATION

                    // IF accept then send ID to server and get actual price and details for payment
                    // ONCE paid then Download biome

                    //TODO GET RESULT AND DOWNLOAD ITEM

                    // get selected ui button
                    var biomeUi = displayingItem.FirstOrDefault(x => x.ID == selectedBiome.Details.ID);

                    Debug.Log("Downloading item");
                    AddressableManager.Instance.DownloadAddressableList(
                        selectedBiome.Details.ID,
                        scenesToCheck,
                        biomeUi!.DownloadProgress,
                        () =>
                        {
                            Debug.Log("Download completed!");

                            //done save to active list
                            BrokerUiActions.TriggerOnDownloadNewBiome(selectedBiome);
                        },
                        OnErrorDownloading);
                }
                else
                {
                    // TODO do a call to api and check if biome is actually owned before proceeding
                    // else remove it from their device

                    Debug.Log("Biome scenes are already downlaoded");
                    displayingItem.FirstOrDefault(x => x.ID == selectedBiome.Details.ID)?.UpdateOwned(true);
                    BrokerUiActions.TriggerOnDownloadNewBiome(selectedBiome);
                }
            }
        }

        private void OnErrorDownloading(string errorMessage)
        {
            Debug.LogError($"There was a problem downloading the biome: {errorMessage}");
        }

        private void OnFilterChange(int val)
        {
            if (val == _activeFilterIndex) return;
            _activeFilterIndex = val;
            if (val == 0) // show all
            {
                displayingItem.ForEach(x => x.gameObject.SetActive(true));
                return;
            }

            if (val == 1) // Owned
            {
                displayingItem.ForEach(x => x.gameObject.SetActive(x.IsOwned));
                return;
            }

            if (val == 2) // Owned
            {
                displayingItem.ForEach(x => x.gameObject.SetActive(x.IsFree));
                return;
            }

            displayingItem.ForEach(x => x.gameObject.SetActive(false));
            int index = 0;
            foreach (var b in _manager.availableBiomes.Biomes)
            {
                var t = Enum.Parse<ECategories>(dropdownFilter.options[val].text);
                if (b.Details.Tags.Contains(t)) continue;
                displayingItem[index].gameObject.SetActive(true);
                index++;
            }
        }
    }
}