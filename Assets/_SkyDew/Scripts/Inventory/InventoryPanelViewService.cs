using System.Collections.Generic;
using _SkyDew.Scripts.Common;
using _SkyDew.Scripts.Common.UI;
using _SkyDew.Scripts.GameItems;
using _SkyDew.Scripts.System;
using AppBootstrap.Runtime;
using UniRx;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using NotImplementedException = System.NotImplementedException;

namespace _SkyDew.Scripts.Inventory
{
    [Injectable]
    public class InventoryPanelViewService : IInventorySub
    {
        [Inject] private IUIService _uiService;
        [Inject] private IInventoryDataService _inventoryDataService;
        [Inject] private IGameItemConfigsService _giConfigService;
        [Inject] private ICameraService _cameraService;
        [Inject] private InventoryPanelViewItem _ghostPrefab;

        private InventoryPanelView _view;
        private DragGhostNode _dragGhostNode;
        private CompositeDisposable _scrollDispose = new();

        [Init(InitSteps.Preload)]
        private void Init()
        {
            // View
            _view = _uiService.GetElement<InventoryPanelView>();
            _view.SetDragActions(AtStartDrag, AtDrag, AtEndDrag);
            // Ghost
            var ghostInst = Object.Instantiate(_ghostPrefab, _view.transform);
            ghostInst.gameObject.SetActive(false);
            _dragGhostNode = new DragGhostNode(ghostInst);
        }

        [Init(InitSteps.Postload)]
        private void SetupViews()
        {
            var dataList = _inventoryDataService.GetItemsData();
            var cnt = Mathf.Min(_view.Items.Count, dataList.Count);
            for (var i = 0; i < cnt; i++)
            {
                if (dataList[i].IsEmpty)
                {
                    SetSlotEmpty(i);
                }

                SetItem(i, dataList[i].Data, dataList[i].Amount);
            }
        }


        //____ IInventorySub __________________________________

        public void AtInventorySlotChange(IList<InventorySlotChangeInfo> changesList)
        {
            foreach (var change in changesList)
            {
                if (change.BecomesEmpty)
                {
                    SetSlotEmpty(change.Index);
                    return;
                }

                if (change.OnlyAmountChange)
                {
                    SetItemAmount(change.Index, change.NewData, change.NewAmount);
                }

                SetItem(change.Index, change.NewData, change.NewAmount);
            }
        }

        //____ MISC __________________________________
        private void AtStartDrag(int id, InventoryPanelViewItem item, PointerEventData ped)
        {
            var items = _inventoryDataService.GetItemsData();
            var data = items[id];
            _dragGhostNode.Transform.position = item.transform.position;
            _dragGhostNode.SetData(data);
            
            if (ped.button == PointerEventData.InputButton.Right)
            {
                _dragGhostNode.SetAmount(1);
            }
            _dragGhostNode.Item.SetSprite(item.IconImage.sprite);
            _dragGhostNode.SetActive(true);
            Observable.EveryUpdate().Subscribe(AtScrollUpdate).AddTo(_scrollDispose);

            void AtScrollUpdate(long _)
            {
                if (!_dragGhostNode.Active)
                    return;

                if (Input.mouseScrollDelta.y > Mathf.Epsilon)
                {
                    _dragGhostNode.AddAmount(1);
                    return;
                }

                if (Input.mouseScrollDelta.y < -Mathf.Epsilon)
                {
                    _dragGhostNode.AddAmount(-1);
                }
            }
        }

        private void AtDrag(int id, InventoryPanelViewItem item, PointerEventData ped)
        {
            if (!_dragGhostNode.Active)
                return;

            var pos = _cameraService.Camera.ScreenToWorldPoint(ped.position);
            pos.z = 0;
            _dragGhostNode.Transform.position = pos;
        }

        private void AtEndDrag(int id, InventoryPanelViewItem item, PointerEventData ped)
        {
            _scrollDispose.Clear();
            _dragGhostNode.SetActive(false);
            if (!_view.IsPointerOverItem(out var newId, out _))
                return;
            if (newId == id)
                return;

            var changeProcess = GetChangeProcess(id, newId);
            _inventoryDataService.ApplyChangeProcess(changeProcess);
        }

        private InventoryChangeProcess GetChangeProcess(int oldId, int newId)
        {
            var changeProcess = _inventoryDataService.GetChangeProcess();
            var oldItem = changeProcess.OldItems[oldId];
            var newItem = changeProcess.OldItems[newId];

            // Change Amount
            if (oldItem.Data == newItem.Data)
            {
                var config = _giConfigService.GetConfig(oldItem.Data.ItemID);
                var maxStack = config.MaxStack;
                var canAdd = maxStack - newItem.Amount;
                if (canAdd == 0)
                    return changeProcess;

                var toAdd = Mathf.Min(canAdd, _dragGhostNode.Amount);
                changeProcess.SetItemAmount(newId, newItem.Amount + toAdd);
                changeProcess.SetItemAmount(oldId, oldItem.Amount - toAdd);
                return changeProcess;
            }

            // Replace or split
            if (newItem.IsEmpty)
            {
                var rest = oldItem.Amount - _dragGhostNode.Amount;
                changeProcess.SetItem(newId, oldItem.Data, _dragGhostNode.Amount);
                changeProcess.SetItemAmount(oldId, rest);
                return changeProcess;
            }

            // Swap
            changeProcess.SetItem(newId, oldItem.Data, oldItem.Amount);
            changeProcess.SetItem(oldId, newItem.Data, newItem.Amount);
            return changeProcess;
        }


        private void ValidateList(IReadOnlyList<GameItemDataAmount> itemsData)
        {
            for (var i = 0; i < itemsData.Count; i++)
            {
                if (itemsData[i] == null)
                {
                    SetSlotEmpty(i);
                    continue;
                }

                SetItem(i, itemsData[i].Data, itemsData[i].Amount);
            }
        }

        private void SetSlotEmpty(int slot)
        {
            var item = _view.Items[slot];
            item.SetIconActive(false);
            item.SetAmountActive(false);
        }

        private void SetItem(int slot, GameItemData data, int count)
        {
            var item = _view.Items[slot];
            var isEmpty = data == null;
            item.SetIconActive(!isEmpty);
            item.SetAmountActive(!isEmpty);
            if (isEmpty)
                return;

            var config = _giConfigService.GetConfig(data.ItemID);
            var sprite = config.Icon;

            item.SetSprite(sprite);
            SetItemAmount(slot, count, config);
        }

        private void SetItemAmount(int slot, GameItemData data, int count)
        {
            var config = _giConfigService.GetConfig(data.ItemID);
            SetItemAmount(slot, count, config);
        }

        private void SetItemAmount(int slot, int count, GameItemConfig config)
        {
            var item = _view.Items[slot];
            if (config.MaxStack == 1)
                item.SetAmountActive(false);
            item.SetAmount(count);
        }


        private class DragGhostNode
        {
            private Transform _dragTr;
            private InventoryPanelViewItem _item;
            private GameItemDataAmount _data = null;
            private bool _active = false;
            private int _amount = 0;

            public DragGhostNode(InventoryPanelViewItem item)
            {
                _item = item;
                _dragTr = item.transform;
            }

            public Transform Transform => _dragTr;
            public InventoryPanelViewItem Item => _item;
            public bool Active => _active;
            public int Amount =>_amount;
            

            public GameItemDataAmount Data
            {
                get => _data;
                set => SetData(value);
            }

            public void SetData(GameItemDataAmount data)
            {
                _data = data;
                _amount = _data.Amount;
                Item.SetAmount(_amount);
            }

            public void SetActive(bool active)
            {
                _item.gameObject.SetActive(active);
                _active = active;
            }

            public void AddAmount(int toAdd)
            {
                _amount = Mathf.Clamp(_amount + toAdd, 1, _data.Amount);
                _item.SetAmount(_amount);
            }
            public void SetAmount(int amount)
            {
                _amount = amount;
                _item.SetAmount(_amount);
            }
        }
    }
}