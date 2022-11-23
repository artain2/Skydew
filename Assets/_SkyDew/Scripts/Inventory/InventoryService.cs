using System.Collections.Generic;
using System.Linq;
using _SkyDew.Scripts.GameItems;
using _SkyDew.Scripts.System;
using AppBootstrap.Runtime;
using UnityEngine;

namespace _SkyDew.Scripts.Inventory
{
    // public interface IInventoryService
    // {
    //     IReadOnlyList<GameItemDataCount> GetItems();
    //     bool TryAddItem(GameItemData data, int amount, out int rest);
    //     void RemoveItem(GameItemData data, int amount);
    //     int GetCount(GameItemData data);
    //     void Swap(int a, int b);
    // }



    [Injectable]
    public class InventoryService //: IInventoryService
    {
        // [Inject] private IGameItemConfigsService _giConfigs;
        // [Inject] private IInventoryDataService _dataService;
        //
        // private List<GameItemDataCount> _items;
        //
        // [Init(InitSteps.Postload)]
        // private void Init()
        // {
        //     
        // }
        //
        // public IReadOnlyList<GameItemDataCount> GetItems() => _items;
        //
        // public bool TryAddItem(GameItemData data, int amount, out int rest)
        // {
        //     var key = data.ItemID;
        //     var config = _giConfigs.GetConfig(key);
        //     rest = amount;
        //     var maxStack = config.MaxStack;
        //
        //     // Add to same data slots
        //     var usedSlots = _items.Where(x => x.Data!=null && x.Data.ItemID == key).ToArray();
        //     for (var i = 0; i < usedSlots.Length; i++)
        //     {
        //         var usedSlot = usedSlots[i];
        //         var toAdd = maxStack - usedSlot.Amount;
        //         if (toAdd == 0)
        //             continue;
        //
        //         var prevVal = usedSlot.Amount;
        //         usedSlot.Amount += toAdd;
        //         rest -= toAdd;
        //         _subs.ForEach(x =>
        //             x.AtInventorySlotChange(i, data, prevVal, data, usedSlot.Amount));
        //         if (rest != 0)
        //             continue;
        //         return true;
        //     }
        //
        //     // Add to empty slots
        //     for (var i = 0; i < _items.Count; i++)
        //     {
        //         if (_items[i].Data != null)
        //             continue;
        //         var newSlotAmount = Mathf.Min(maxStack, rest);
        //         _items[i] = new GameItemDataCount(data, newSlotAmount);
        //         rest -= newSlotAmount;
        //         _subs.ForEach(x =>
        //             x.AtInventorySlotChange(i, null, 0, data, newSlotAmount));
        //         if (rest != 0)
        //             continue;
        //         return true;
        //     }
        //
        //     // Return rest
        //     return false;
        // }
        //
        // public void RemoveItem(GameItemData data, int amount)
        // {
        //     var rest = amount;
        //     for (var i = _items.Count - 1; i >= 0; i--)
        //     {
        //         if (_items[i] == null || _items[i].Data.ItemID != data.ItemID)
        //             continue;
        //         // Remove slot
        //         var itemAmount = _items[i].Amount;
        //         if (rest >= itemAmount)
        //         {
        //             rest -= itemAmount;
        //             _items[i] = null;
        //             _subs.ForEach(x =>
        //                 x.AtInventorySlotChange(i, data, itemAmount, null, 0));
        //             if (rest != 0)
        //                 continue;
        //
        //             return;
        //         }
        //
        //         // Remove part
        //         _items[i].Amount -= rest;
        //         _subs.ForEach(x =>
        //             x.AtInventorySlotChange(i, data, itemAmount, data, _items[i].Amount));
        //         return;
        //     }
        // }
        //
        // public int GetCount(GameItemData data)
        // {
        //     return _items
        //         .Where(x => x != null && x.Data.ItemID == data.ItemID)
        //         .Sum(x => x.Amount);
        // }
        //
        // public void Swap(int a, int b)
        // {
        //     (_items[a], _items[b]) = (_items[b], _items[a]);
        //     _subs.ForEach(x =>
        //         x.AtInventorySlotChange(a, _items[b].Data, _items[b].Amount, _items[a].Data, _items[a].Amount));
        //     _subs.ForEach(x =>
        //         x.AtInventorySlotChange(b, _items[a].Data, _items[a].Amount ,_items[b].Data, _items[b].Amount));
        // }
    }
}