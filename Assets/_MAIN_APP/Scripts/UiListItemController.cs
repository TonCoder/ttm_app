using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace _MAIN_APP.Scripts
{
    public class UiListItemController : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI itemTextMesh;
        [SerializeField] private Image itemImage;

        public void SetItemValues(string label, Sprite itemImg)
        {
            itemTextMesh.text = label;
            itemImage.sprite = itemImg;
        }
    }
}