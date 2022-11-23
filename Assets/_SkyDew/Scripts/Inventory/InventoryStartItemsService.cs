using System;
using _SkyDew.Scripts.Common;
using _SkyDew.Scripts.GameItems;
using _SkyDew.Scripts.System;
using AppBootstrap.Runtime;
using NotImplementedException = System.NotImplementedException;

namespace _SkyDew.Scripts.Inventory
{
    [Injectable]
    public class InventoryStartItemsService : IDataService
    {
        [Inject] private IInventoryDataService _inventoryData;

        private Data _data;

        [Init(InitSteps.Run)]
        private void Init()
        {
            if (_data.Inited)
                return;

            _inventoryData.SetItem(0, new GameItemData("Wood"), 5);
            _inventoryData.SetItem(1, new GameItemData("Apple"), 5);
            _data.Inited = true;
        }

        // ____ IDataService ______________________________________________
        public string GetDataKey() => "InventoryStartItemsService";

        public void SetData(object data) => _data = data as Data;

        public object GetData() => _data;

        public object CreateData() => new Data();

        [Serializable]
        private class Data
        {
            public bool Inited = false;
        }
    }
}