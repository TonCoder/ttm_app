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
    public class StoreSectionController : MonoBehaviour
    {
        [Header("Sections Setup")] [SerializeField]
        private SoBrokerUiActions uiBroker;

        [SerializeField] private Transform uiListContainer;

        [SerializeField] private TMP_Dropdown dropdownFilter;

        [SerializeField] private UIExpansionPopUpController expansionPopUpController;

        [SerializeField] private GameObject uiButtonPrefab;

        [SerializeField] private UnityEvent onExpansionSelected;

        [SerializeField, Space(10)] private List<UiItemController> displayingItem = new List<UiItemController>();

        private int _activeFilterIndex;
        private GameManager _manager;
        private SoExpansionDetails _selectedExpansion;

        private void Start()
        {
            _manager = GameManager.Instance;
            uiBroker.OnResetStore += ItemDeletedResetStore;
            dropdownFilter.options.Clear();
            Init();
        }

        private void OnDisable()
        {
            uiBroker.OnResetStore -= ItemDeletedResetStore;
        }

        private void ItemDeletedResetStore()
        {
            Init();
        }

        public void Init()
        {
            if (_manager?.AvailableExpansionInst == null) return;

            SetupDropDownList();
            // create the item to the list
            for (var i = 0; i < _manager.AvailableExpansionInst.Expansions.Count; i++)
            {
                var expansion = _manager.AvailableExpansionInst.Expansions[i];
                if (_manager.ownedExpansions.Expansions.Any(x => x.Details.ID == expansion.Details.ID))
                {
                    expansion.IsActive = true;
                }

                if ((displayingItem.Count - 1) >= i)
                {
                    displayingItem[i].SetDisplayData(expansion.Details, expansion.IsActive ? null : ShowBreakdownPopUp);
                }
                else
                {
                    CreateUiButton(expansion);
                }
            }
        }

        private void SetupDropDownList()
        {
            // setup filter list
            dropdownFilter.onValueChanged.AddListener(OnFilterChange);

            foreach (var categories in _manager.AvailableExpansionInst.GetTags)
            {
                // setup dropdown options based on the filter
                dropdownFilter.options.Add(new TMP_Dropdown.OptionData(categories.ToString()));
            }

            dropdownFilter.options.Sort((a, b) => string.Compare(a.text, b.text, StringComparison.Ordinal));

            dropdownFilter.options.Insert(0, new TMP_Dropdown.OptionData("Free"));
            dropdownFilter.options.Insert(0, new TMP_Dropdown.OptionData("Owned"));
            dropdownFilter.options.Insert(0, new TMP_Dropdown.OptionData("Show All"));

            dropdownFilter.captionText.text = "Show All";
            dropdownFilter.value = _activeFilterIndex;
        }

        private void CreateUiButton(in SoExpansionDetails expansion)
        {
            var item = Instantiate(uiButtonPrefab, uiListContainer).GetComponent<UiItemController>();
            item.SetDisplayData(expansion.Details.Price <= 0,
                expansion.IsActive,
                expansion.Details,
                expansion.IsActive ? null : ShowBreakdownPopUp);
            displayingItem.Add(item);
        }

        private void ShowBreakdownPopUp(int id)
        {
            if (_manager.AvailableExpansionInst.GetExpansionById(id, out _selectedExpansion))
            {
                Debug.Log("Store Expansion Selected");
                expansionPopUpController.SetPupUpInfo(_selectedExpansion);
                onExpansionSelected?.Invoke();
            }
        }

        public void OnPurchaseSelected()
        {
            DownloadExpansion(_selectedExpansion.Details.ID);
        }

        private void DownloadExpansion(int id)
        {
            // perform download on the object with addressable
            if (_manager.AvailableExpansionInst.GetExpansionById(id, out var expansion))
            {
                var scenesToDownload = expansion.audioTracks?.Select(x => x?.AudioReference).ToArray();

                if (!AddressableManager.WereAssetsDownloaded(scenesToDownload))
                {
                    Debug.Log("Need to download expansion scenes");
                    //TODO SHOW PURCHASE VALIDATION

                    // IF accept then send ID to server and get actual price and details for payment
                    // ONCE paid then Download expansion

                    //TODO GET RESULT AND DOWNLOAD ITEM

                    // get selected ui button
                    var selectedItemUi = displayingItem.FirstOrDefault(x => x.ID == expansion.Details.ID);
                    Debug.Log("Downloading item");
                    AddressableManager.Instance.DownloadAddressableList(
                        expansion.Details.ID,
                        scenesToDownload,
                        selectedItemUi!.DownloadProgress,
                        () =>
                        {
                            Debug.Log("Download completed!");
                            selectedItemUi?.UpdateOwned(true);

                            //done save to active list
                            uiBroker.TriggerOnDownloadExpansion(expansion);
                        },
                        OnErrorDownloading);
                }
                else
                {
                    // TODO do a call to api and check if expansion is actually owned before proceeding
                    // else remove it from their device

                    Debug.Log("Expansion tracks are already downloaded");
                    displayingItem.FirstOrDefault(x => x.ID == expansion.Details.ID)?.UpdateOwned(true);
                    uiBroker.TriggerOnDownloadExpansion(expansion);
                }
            }
        }

        private void OnErrorDownloading(string errorMessage)
        {
            Debug.LogError($"There was a problem downloading the expansion: {errorMessage}");
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
                displayingItem.ForEach(x => x.gameObject.SetActive(x.IsActive));
                return;
            }

            if (val == 2) // Owned
            {
                displayingItem.ForEach(x => x.gameObject.SetActive(x.IsFree));
                return;
            }

            displayingItem.ForEach(x => x.gameObject.SetActive(false));
            var category = Enum.Parse<ECategories>(dropdownFilter.options[val].text);

            for (var i = 0; i < _manager.AvailableExpansionInst.Expansions.Count; i++)
            {
                if (!_manager.AvailableExpansionInst.Expansions[i].Details.Tags.Contains(category)) continue;
                displayingItem[i].gameObject.SetActive(true);
            }
        }
    }
}