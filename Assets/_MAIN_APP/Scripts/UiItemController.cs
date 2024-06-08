using System;
using _MAIN_APP.Scripts.Models;
using CreativeVeinStudio.Simple_Dialogue_System.Attributes;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace _MAIN_APP.Scripts
{
    [RequireComponent(typeof(Button), typeof(Image))]
    public class UiItemController : MonoBehaviour
    {
        [FieldTitle("General Settings")] [SerializeField]
        private TextMeshProUGUI _title;

        [SerializeField] private TextMeshProUGUI _artist;
        [SerializeField] private Image bgImage;
        [SerializeField] private TextMeshProUGUI _qty;
        [SerializeField] private Button thisBtn;
        [SerializeField] private Canvas freeTag;
        [SerializeField] private Image ownedOverlay;
        [SerializeField] private Canvas ownedTag;
        [SerializeField] private Canvas priceTag;
        [SerializeField] private TextMeshProUGUI priceTagText;

        [FieldTitle("General Settings")] [SerializeField]
        private TextMeshProUGUI _progressText;

        [SerializeField] private Image _progressImage;
        [SerializeField] private GameObject[] hideOnDownload;

        private UIDisplayData _displayData;
        private bool _isFree;
        private bool _isActive;
        private int _itemId;

        public bool IsFree => _isFree;
        public bool IsActive => _isActive;
        public int ID => _itemId;

        public void SetDisplayData(bool isFree, bool isActive, UIDisplayData data, Action<int> btnAction = null)
        {
            _isFree = isFree;
            _isActive = isActive;
            if (thisBtn) thisBtn.enabled = !isActive;
            priceTag.enabled = !isActive && !isFree;
            priceTagText.text = $"${data.Price}";
            SetDisplayData(data, isActive ? null : btnAction);
        }

        public void SetDisplayData(UIDisplayData data, Action<int> btnAction = null)
        {
            // _displayData = new UIDisplayData(data.ID, data.Title, data.Artist, data.Qty, data.ItemImage);
            _displayData = data;
            _itemId = data.ID;

            //update ui with new info
            UpdateContent();
            if (btnAction != null)
            {
                thisBtn.onClick.AddListener(() => btnAction?.Invoke(_displayData.ID));
            }
        }

        public void DownloadProgress(float progress)
        {
            if (progress >= 1)
            {
                //its done downloading, hide downlaod info and set OWNED tag
                foreach (var o in hideOnDownload)
                {
                    o.SetActive(true);
                }

                ownedTag.enabled = true;
                if (ownedOverlay) ownedOverlay.enabled = true;
                _progressImage.enabled = false;
                _progressText.enabled = false;
                return;
            }

            if (!_progressImage.enabled)
            {
                foreach (var o in hideOnDownload)
                {
                    o.SetActive(false);
                }

                _progressImage.enabled = true;
                _progressText.enabled = true;
            }

            _progressImage.fillAmount = progress;
            _progressText.text = $"{progress}%";
        }

        internal void UpdateOwned(bool owned)
        {
            if (ownedTag)
            {
                if (thisBtn) thisBtn.enabled = !owned;
                if (ownedOverlay) ownedOverlay.enabled = true;
                ownedTag.enabled = owned;
                _isActive = owned;
            }
        }

        private void UpdateContent()
        {
            if (_title) _title.text = _displayData.Title;
            if (_artist) _artist.text = _displayData.Artist;
            if (bgImage) bgImage.sprite = _displayData.ItemImage ?? bgImage.sprite;
            if (_qty) _qty.text = $"Scene qty: {_displayData.Qty}";
            if (_isFree && freeTag)
                freeTag.enabled = true;
            if (IsActive && ownedTag)
                ownedTag.enabled = true;
            if (ownedOverlay) ownedOverlay.enabled = IsActive;
        }
    }
}