using System;
using System.Collections.Generic;
using System.Linq;
using _MAIN_APP.Scripts.ScriptableObjects;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;

namespace _MAIN_APP.Scripts
{
    public class UIExpansionPopUpController : MonoBehaviour
    {
        [FormerlySerializedAs("uiExpansionItem")] [SerializeField]
        private UiItemController uiHeaderItem;

        [SerializeField] private TextMeshProUGUI uiDesciptionTextMesh;

        [SerializeField] private Transform uiListContainer;

        [SerializeField] private UiListItemController uiListItemPrefab;

        [SerializeField, Tooltip("Has a capacity of 20")]
        private List<UiListItemController> listItems = new List<UiListItemController>(20);

        public void SetPupUpInfo(SoExpansionDetails expansion)
        {
            uiHeaderItem.SetDisplayData(false, false, expansion.Details);
            uiDesciptionTextMesh.text = expansion.Details.Description;
            if (listItems.Count >= expansion.audioTracks.Count)
            {
                listItems.ForEach(x => x.gameObject.SetActive(false));
                for (int i = 0; i < expansion.audioTracks.Count; i++)
                {
                    listItems[i].SetItemValues(expansion.audioTracks[i]?.details.Title,
                        expansion.audioTracks[i]?.details.ItemImage);
                    listItems[i].gameObject.SetActive(true);
                }

                return;
            }

            for (int i = 0; i < expansion.audioTracks.Count; i++)
            {
                if (listItems.Count > i)
                {
                    listItems[i].SetItemValues(expansion.audioTracks[i]?.details.Title,
                        expansion.audioTracks[i]?.details.ItemImage);
                    listItems[i].gameObject.SetActive(true);
                }
                else
                {
                    listItems.Add(Instantiate(uiListItemPrefab, uiListContainer).GetComponent<UiListItemController>());
                    listItems.Last().SetItemValues(expansion.audioTracks[i]?.details.Title,
                        expansion.audioTracks[i]?.details.ItemImage);
                }
            }
        }
    }
}