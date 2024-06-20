using System;
using _MAIN_APP.Scripts.Models;
using CreativeVeinStudio.Simple_Dialogue_System.Attributes;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;
using Random = UnityEngine.Random;

namespace _MAIN_APP.Scripts
{
    [RequireComponent(typeof(Button), typeof(Image))]
    public class UiItemController : MonoBehaviour
    {
        [FieldTitle("General Settings")] [SerializeField]
        private TextMeshProUGUI _title;

        [SerializeField] private Button deleteBtn;
        [SerializeField] private Button confirmDeleteBtn;
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
        private int _itemId;

        public bool IsFree => _isFree;
        public int ID => _itemId;
        public bool IsOwned { get; internal set; }

        public void SetDisplayData(bool isFree, bool isOwned, UIDisplayData data, Action<int> btnAction = null)
        {
            _isFree = isFree;
            IsOwned = isOwned;
            if (isOwned && ownedTag) ownedTag.enabled = true;
            if (ownedOverlay) ownedOverlay.enabled = isOwned;

            deleteBtn?.gameObject.SetActive(false);
            if (thisBtn) thisBtn.enabled = !isOwned;

            if (priceTag) priceTag.enabled = !isOwned && !isFree;
            if (priceTagText) priceTagText.text = $"${data.Price}";
            SetDisplayData(data, isOwned ? null : btnAction);
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
                thisBtn.onClick.RemoveAllListeners();
                thisBtn.onClick.AddListener(() => btnAction?.Invoke(_displayData.ID));
            }
        }

        public void SetDisplayData(UIDisplayData data, Action<int> btnAction, Action<int> deleteAction)
        {
            deleteBtn?.gameObject.SetActive(deleteAction != null);
            if (deleteAction != null)
            {
                confirmDeleteBtn.onClick.RemoveAllListeners();
                confirmDeleteBtn.onClick.AddListener(() => deleteAction?.Invoke(data.ID));
            }

            SetDisplayData(data, btnAction);
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
            }
        }

        private void UpdateContent()
        {
            if (_title) _title.text = _displayData.Title;
            if (_artist) _artist.text = _displayData.Artist;
            if (bgImage) bgImage.sprite = _displayData.ItemImage ?? bgImage.sprite;
            if (!_displayData.ItemImage && bgImage) bgImage.color = Random.ColorHSV(0f, 1f, 1f, 1f, 0.5f, 1f);
            if (_qty) _qty.text = $"Track qty: {_displayData.Qty}";
            if (_isFree && freeTag) freeTag.enabled = true;
        }
    }
}