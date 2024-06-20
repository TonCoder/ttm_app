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
        private string[] _extraDropDownOptions = new[] { "Free", "Owned" };

        private SoExpansionDetails _tempExpansion;
        private bool _tempOwned;

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
                _tempExpansion = _manager.AvailableExpansionInst.Expansions[i];
                _tempOwned = _manager.ownedExpansions.Expansions.Any(x => x.Details.ID == _tempExpansion.Details.ID);

                if ((displayingItem.Count - 1) >= i)
                {
                    displayingItem[i].SetDisplayData(_tempExpansion.Details.Price <= 0,
                        _tempOwned,
                        _tempExpansion.Details,
                        _tempOwned ? null : ShowBreakdownPopUp);
                }
                else
                {
                    CreateUiButton(ref _tempExpansion, ref _tempOwned);
                }
            }
        }

        private void SetupDropDownList()
        {
            // setup filter list
            dropdownFilter.onValueChanged.AddListener(OnFilterChange);
            TtmUtilities.SetupDropdownList(ref dropdownFilter, _manager.ownedExpansions.GetTags, _extraDropDownOptions,
                OnFilterChange);
        }

        private void CreateUiButton(ref SoExpansionDetails expansion, ref bool owned)
        {
            var item = Instantiate(uiButtonPrefab, uiListContainer).GetComponent<UiItemController>();
            item.SetDisplayData(expansion.Details.Price <= 0,
                owned,
                expansion.Details,
                owned ? null : ShowBreakdownPopUp);
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
                var tracksToDownload = expansion.audioTracks?.Select(x => x?.AudioReference).ToArray();

                if (!AddressableManager.WereAssetsDownloaded(tracksToDownload))
                {
                    Debug.Log("Need to download expansion scenes");
                    //TODO SHOW PURCHASE VALIDATION

                    // IF accept then send ID to server and get actual price and details for payment
                    // ONCE paid then Download expansion

                    //TODO GET RESULT AND DOWNLOAD ITEM

                    // get selected ui button
                    var selectedItemUi = displayingItem.FirstOrDefault(x => x.ID == expansion.Details.ID);

#if UNITY_EDITOR
                    Debug.Log("Downloading item");
#endif
                    AddressableManager.Instance.DownloadList(
                        expansion.Details.ID,
                        tracksToDownload,
                        selectedItemUi!.DownloadProgress,
                        () =>
                        {
#if UNITY_EDITOR
                            Debug.Log("Download completed!");
#endif
                            selectedItemUi?.UpdateOwned(true);

                            //done save to active list
                            uiBroker.TriggerOnDownloadExpansion(expansion);

                            //release downloaded from memory
                            AddressableManager.Instance.ReleaseAssetList(ref tracksToDownload);
                        },
                        OnErrorDownloading);
                }
                else
                {
                    // TODO do a call to api and check if expansion is actually owned before proceeding
                    // else remove it from their device

#if UNITY_EDITOR
                    Debug.Log("Expansion tracks are already downloaded");
#endif
                    displayingItem.FirstOrDefault(x => x.ID == expansion.Details.ID)?.UpdateOwned(true);
                    uiBroker.TriggerOnDownloadExpansion(expansion);
                }
            }
        }

        private void OnErrorDownloading(string errorMessage)
        {
#if UNITY_EDITOR
            Debug.LogError($"There was a problem downloading the expansion: {errorMessage}");
#endif
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
            var category = Enum.Parse<ECategories>(dropdownFilter.options[val].text);

            for (var i = 0; i < _manager.AvailableExpansionInst.Expansions.Count; i++)
            {
                if (!_manager.AvailableExpansionInst.Expansions[i].Details.Tags.Contains(category)) continue;
                displayingItem[i].gameObject.SetActive(true);
            }
        }
    }
}