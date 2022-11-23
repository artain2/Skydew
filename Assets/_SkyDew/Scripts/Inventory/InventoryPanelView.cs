using System;
using System.Collections.Generic;
using Custom.Extentions;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

namespace _SkyDew.Scripts.Inventory
{
    public class InventoryPanelView : MonoBehaviour
    {
        [SerializeField] private List<InventoryPanelViewItem> items;

        private Action<int, InventoryPanelViewItem, PointerEventData> _startDragAction;
        private Action<int, InventoryPanelViewItem, PointerEventData> _dragAction;
        private Action<int, InventoryPanelViewItem, PointerEventData> _endDragAction;

        private bool _isFocusedOnItem = false;
        private InventoryPanelViewItem _focusedItem;
        private int _focusedItemIndex;

        private void Start()
        {
            for (var i = 0; i < items.Count; i++)
            {
                var index = i;
                var item = items[i];
                var et = item.EventTrigger;

                et.AddTrigger(EventTriggerType.BeginDrag, bed =>
                    _startDragAction?.Invoke(index, item, bed as PointerEventData));
                et.AddTrigger(EventTriggerType.Drag, bed =>
                    _dragAction?.Invoke(index, item, bed as PointerEventData));
                et.AddTrigger(EventTriggerType.EndDrag, bed =>
                    _endDragAction?.Invoke(index, item, bed as PointerEventData));
                et.AddTrigger(EventTriggerType.PointerEnter, _ =>
                {
                    _isFocusedOnItem = true;
                    _focusedItem = item;
                    _focusedItemIndex = index;
                });
                et.AddTrigger(EventTriggerType.PointerExit, _ => { _isFocusedOnItem = false; });
            }
        }

        public IReadOnlyList<InventoryPanelViewItem> Items => items;

        public void SetDragActions(
            Action<int, InventoryPanelViewItem, PointerEventData> startDrag,
            Action<int, InventoryPanelViewItem, PointerEventData> drag,
            Action<int, InventoryPanelViewItem, PointerEventData> endDrag)
        {
            _startDragAction = startDrag;
            _dragAction = drag;
            _endDragAction = endDrag;
        }

        public bool IsPointerOverItem(out int index, out InventoryPanelViewItem view)
        {
            view = _focusedItem;
            index = _focusedItemIndex;
            return _isFocusedOnItem;
        }
    }
}