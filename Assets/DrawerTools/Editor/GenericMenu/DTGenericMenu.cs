using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace DrawerTools
{
    public class DTGenericMenu
    {
        private List<Node> items = new List<Node>();
        private Dictionary<string, object[]> _params = new Dictionary<string, object[]>();

        public void Show()
        {
            var menu = new GenericMenu();
            foreach (var x in items)
            {
                if (x.Enabled)
                {
                    menu.AddItem(new GUIContent(x.Item), false, () => x.Callback());
                }
                else
                {
                    menu.AddDisabledItem(new GUIContent(x.Item));
                }

                if (x.AddSeparator)
                {
                    var slashInd = x.Item.LastIndexOf("/");
                    if (slashInd<0)
                    {
                        menu.AddSeparator("");
                    }
                    else
                    {
                        var separatorStr = x.Item.Substring(0, slashInd+1);
                        menu.AddSeparator(separatorStr);
                    }
                }
            }
            menu.ShowAsContext();
        }


        public DTGenericMenu AddItem(string item, Action callback, bool enable = true, bool separate = false)
        {
            items.Add(new Node(item, callback, enable, separate));
            return this;
        }
        
        public static DTGenericMenu New() => new DTGenericMenu();

        private struct Node
        {
            public string Item;
            public Action Callback;
            public bool Enabled;
            public bool AddSeparator;

            public Node(string item, Action callback,  bool enabled, bool addSeparator)
            {
                Item = item;
                Callback = callback;
                Enabled = enabled;
                AddSeparator = addSeparator;
            }
        }
    }
}