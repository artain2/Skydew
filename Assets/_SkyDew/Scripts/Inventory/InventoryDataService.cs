using System;
using System.Collections.Generic;
using _SkyDew.Scripts.Common;
using _SkyDew.Scripts.GameItems;
using AppBootstrap.Runtime;

namespace _SkyDew.Scripts.Inventory
{
    public interface IInventoryDataService
    {
        IReadOnlyList<GameItemDataAmount> GetItemsData();
        void SetItem(int index, GameItemData data, int amount);
        void SetItemAmount(int index, int amount);
        InventoryChangeProcess GetChangeProcess();
        void ApplyChangeProcess(InventoryChangeProcess process);
    }

    public interface IInventorySub
    {
        void AtInventorySlotChange(IList<InventorySlotChangeInfo> changesList);
    }

    [Injectable]
    public class InventoryDataService : IInventoryDataService, IDataService
    {
        [Inject] private List<IInventorySub> _subs;

        private InventoryData _data = new();

        // ____ IInventoryDataService _______________________________

        public IReadOnlyList<GameItemDataAmount> GetItemsData() => _data.items;

        public void SetItem(int index, GameItemData data, int amount)
        {
            var old = _data.items[index];
            _data.items[index] = new GameItemDataAmount(data, amount);
            var changes = new[]
            {
                new InventorySlotChangeInfo(index, old.Data, data, old.Amount, amount)
            };
            _subs.ForEach(x => x.AtInventorySlotChange(changes));
        }

        public void SetItemAmount(int index, int amount)
        {
            var oldAmount = _data.items[index].Amount;
            var data = _data.items[index].Data;
            _data.items[index].Amount = amount;
            var changes = new[]
            {
                new InventorySlotChangeInfo(index, data, oldAmount, amount)
            };
            _subs.ForEach(x => x.AtInventorySlotChange(changes));
        }

        public InventoryChangeProcess GetChangeProcess()
        {
            var process = new InventoryChangeProcess(_data.items);
            return process;
        }

        public void ApplyChangeProcess(InventoryChangeProcess process)
        {
            var changes = process.GetChanges();
            foreach (var change in changes)
                _data.items[change.Index] = change.NewDataAmount;

            _subs.ForEach(x => x.AtInventorySlotChange(changes));
        }

        // ____ IDataService _______________________________

        public string GetDataKey() => "Inventory";

        public void SetData(object data) => _data = data as InventoryData;

        public object GetData() => _data;

        public object CreateData()
        {
            var data = new InventoryData();
            for (var i = 0; i < 10; i++)
                data.items.Add(new GameItemDataAmount(null, 0));
            return data;
        }

        [Serializable]
        public class InventoryData
        {
            public List<GameItemDataAmount> items = new();
        }
    }

    public class InventoryChangeProcess
    {
        private List<GameItemDataAmount> _oldItems;
        private Dictionary<int, GameItemDataAmount> _newItems = new Dictionary<int, GameItemDataAmount>();

        public IReadOnlyList<GameItemDataAmount> OldItems => _oldItems;
        public IReadOnlyDictionary<int, GameItemDataAmount> GetNewItems() => _newItems;

        public InventoryChangeProcess(List<GameItemDataAmount> oldItems)
        {
            _oldItems = oldItems;
        }

        public void SetItemAmount(int index, int amount)
        {
            if (amount == 0)
            {
                ClearItem(index);
                return;
            }
            if (!_newItems.TryGetValue(index, out var da))
            {
                da = _oldItems[index].Copy();
                _newItems[index] = da;
            }

            da.Amount = amount;
        }

        public void SetItem(int index, GameItemData data, int amount)
        {
            _newItems[index] = new GameItemDataAmount(data, amount);
        }

        public void SetItem(int index, GameItemDataAmount dataAmount) =>
            SetItem(index, dataAmount.Data, dataAmount.Amount);

        public void ClearItem(int index)
        {
            _newItems[index] = new GameItemDataAmount(null, 0);
        }

        public IList<InventorySlotChangeInfo> GetChanges()
        {
            List<InventorySlotChangeInfo> result = new();
            foreach (var (index, dataCount) in _newItems)
            {
                var old = _oldItems[index];
                var change = new InventorySlotChangeInfo(index, old, dataCount);
                result.Add(change);
            }

            return result;
        }
    }

    public class InventorySlotChangeInfo
    {
        public int Index { get; }
        public GameItemData OldData { get; }
        public GameItemData NewData { get; }
        public int OldAmount { get; }
        public int NewAmount { get; }
        public bool BecomesEmpty => OldAmount != 0 && NewAmount == 0;
        public bool BecomesFilled => OldAmount == 0 && NewAmount != 0;
        public bool OnlyAmountChange => OldData == NewData;
        public GameItemDataAmount NewDataAmount => new(NewData, NewAmount);

        public InventorySlotChangeInfo(int index, GameItemData oldData, GameItemData newData, int oldAmount,
            int newAmount)
        {
            Index = index;
            OldData = oldData;
            NewData = newData;
            OldAmount = oldAmount;
            NewAmount = newAmount;
        }

        public InventorySlotChangeInfo(int index, GameItemData oldData, int oldAmount, int newAmount) : this(index,
            oldData, oldData, oldAmount, newAmount)
        {
        }

        public InventorySlotChangeInfo(int index, GameItemDataAmount oldData, GameItemDataAmount newData) : this(index,
            oldData.Data, newData.Data, oldData.Amount, newData.Amount)
        {
        }

        public InventorySlotChangeInfo(int index, GameItemDataAmount oldData, int newAmount) : this(index, oldData.Data,
            oldData.Data, oldData.Amount, newAmount)
        {
        }
    }
}