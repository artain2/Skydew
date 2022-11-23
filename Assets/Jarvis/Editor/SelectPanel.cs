using System;
using System.Collections.Generic;
using System.Linq;
using DrawerTools;
using UnityEngine;

namespace Jarvis
{
    public class SelectPanel : DTPanel
    {
        private List<DTButton> _buttons = new List<DTButton>();
        private Action<int> _clickCallback;
        private float _itemWidth = 100f;
        private DTButton _selectedButton;

        public SelectPanel(IDTPanel parent) : base(parent)
        {
            panelBehaviour.DrawScrollInExpand = true;
        }

        protected override void AtDraw()
        {
            for (var i = 0; i < _buttons.Count; i++)
            {
                _buttons[i].Draw();
            }
        }

        public SelectPanel SetItems(IList<string> items)
        {
            _buttons.Clear();
            for (var i = 0; i < items.Count; i++)
            {
                var id = i;
                var btn = new DTButton(items[i], () => AtClick(id));
                btn.SetWidth(_itemWidth);
                var style = GetNormalButtonStyle();
                btn.SetStyle(style);
                _buttons.Add(btn);
            }

            return this;
        }

        public SelectPanel SetItemsWidth(float width)
        {
            _itemWidth = width;
            foreach (var button in _buttons)
            {
                button.SetWidth(width);
            }

            return this;
        }

        public SelectPanel SetClickAction(Action<int> clickCallback)
        {
            _clickCallback = clickCallback;
            return this;
        }

        public SelectPanel SelectButton(string buttonName)
        {
            if (_selectedButton!=null)
            {
                _selectedButton.SetStyle(GetNormalButtonStyle());;
            }
            var toSelect = _buttons.FirstOrDefault(x => x.Name == buttonName);
            if (toSelect == null)
            {
                return this;
            }

            _selectedButton = toSelect;
            _selectedButton.SetStyle(GetSelectedButtonStyle());
            return this;
        }

        private GUIStyle GetSelectedButtonStyle() => new GUIStyle("Tooltip") {fontStyle = FontStyle.BoldAndItalic};
        private GUIStyle GetNormalButtonStyle() => new GUIStyle("Tooltip") {fontStyle = FontStyle.Normal};

        private void AtClick(int id)
        {
            _clickCallback(id);
        }
    }
}