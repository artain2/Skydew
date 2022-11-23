using UnityEngine;

namespace _SkyDew.Scripts.GameItems
{
    public class GameItemConfig : ScriptableObject
    {
        [SerializeField] private string itemID;
        [SerializeField] private int maxStack;
        [SerializeField] private Sprite icon;

        public string ItemID => itemID;

        public int MaxStack => maxStack;

        public Sprite Icon => icon;
    }
}