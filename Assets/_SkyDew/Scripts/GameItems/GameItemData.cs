using System;
using UnityEngine;

namespace _SkyDew.Scripts.GameItems
{
    [Serializable]
    public class GameItemData
    {
        [SerializeField] private string itemID;

        public string ItemID => itemID;

        public GameItemData(string itemID)
        {
            this.itemID = itemID;
        }
    }
    [Serializable]
    public class GameItemDataAmount
    {
        [SerializeField] private GameItemData data;
        [SerializeField] private int amount;

        public bool IsEmpty => data == null || amount == 0;

        public GameItemData Data => data;

        public int Amount
        {
            get => amount;
            set => amount = value;
        }


        public GameItemDataAmount(GameItemData data, int amount)
        {
            this.data = data;
            this.amount = amount;
        }

        public GameItemDataAmount Copy() => new (data, amount);
    }
}