using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace _SkyDew.Scripts.Inventory
{
    public class InventoryPanelViewItem : MonoBehaviour
    {
        [SerializeField] private Image iconImage;
        [SerializeField] private Image backImage;
        [SerializeField] private TMP_Text amountLabel;
        [SerializeField] private EventTrigger eventTrigger;

        public EventTrigger EventTrigger => eventTrigger;
        public Image IconImage => iconImage;

        public void SetSprite(Sprite sprite) => iconImage.sprite = sprite;
        public void SetAmount(int amount) => amountLabel.text = amount.ToString();
        public void SetIconActive(bool active) => iconImage.gameObject.SetActive(active);
        public void SetAmountActive(bool active) => amountLabel.gameObject.SetActive(active);

    }
}