using System;
using System.Collections.Generic;
using System.Linq;
using _MAIN_APP.Scripts.ScriptableObjects;
using UnityEngine;
using UnityEngine.Serialization;

namespace _MAIN_APP.Scripts
{
    public class UIExpansionPopUpController : MonoBehaviour
    {
        [FormerlySerializedAs("Setup")] [SerializeField]
        private Transform uiListContainer;

        [SerializeField] private UiListItemController uiListItemPrefab;
        [SerializeField] private UiItemController uiExpansionItem;

        [SerializeField, Tooltip("Has a capacity of 10")]
        private List<UiListItemController> listItems = new List<UiListItemController>(20);

        public void SetPupUpInfo(SoExpansionDetails expansion)
        {
            uiExpansionItem.SetDisplayData(expansion.IsFree, expansion.IsActive, expansion.Details);

            if (listItems.Count >= expansion.audioScenes.Count)
            {
                listItems.ForEach(x => x.gameObject.SetActive(false));
                for (int i = 0; i < expansion.audioScenes.Count; i++)
                {
                    listItems[i].SetItemValues(expansion.audioScenes[i]?.details.Title,
                        expansion.audioScenes[i]?.details.ItemImage);
                    listItems[i].gameObject.SetActive(true);
                }

                return;
            }

            for (int i = 0; i < expansion.audioScenes.Count; i++)
            {
                if (listItems.Count > i)
                {
                    listItems[i].SetItemValues(expansion.audioScenes[i]?.details.Title,
                        expansion.audioScenes[i]?.details.ItemImage);
                    listItems[i].gameObject.SetActive(true);
                }
                else
                {
                    listItems.Add(Instantiate(uiListItemPrefab, uiListContainer).GetComponent<UiListItemController>());
                    listItems.Last().SetItemValues(expansion.audioScenes[i]?.details.Title,
                        expansion.audioScenes[i]?.details.ItemImage);
                }
            }
        }
    }
}