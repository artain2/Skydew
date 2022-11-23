using System;
using System.Collections.Generic;
using System.Reflection;
using AppBootstrap.Editor.Jarvis.Utils;
using DrawerTools;
using UnityEngine;

namespace AppBootstrap.Editor.Jarvis.Listeners
{
    public class ListenersDrawer : DTPanel
    {
        public Type TargetType { get; private set; }
        public FieldInfo TargetField { get; private set; }
        public List<string> InjectionOrder { get; private set; }

        private DTListDrawer<string, DTObject<TextAsset>> _listDrawer;
        private DTExpandToggle _expand = new DTExpandToggle();
        private DTObject<TextAsset> _classLabel = new DTObject<TextAsset>("");
        private DTLabel _fieldLabel = new DTLabel("");
        private DTLabel _amountLabel = new DTLabel("");
        private InjectionFilesCache _csCache;
        private bool _isInited = false;
        private List<string> _injections;


        public ListenersDrawer(IDTPanel parent) : base(parent)
        {
            _csCache = InjectionCacheUtils.GetConfig();
            _listDrawer = new DTListDrawer<string, DTObject<TextAsset>>(this);
            _listDrawer.DisableAdd();
            _listDrawer.SetRemoveEnabled(false);
            _listDrawer.DisableTitle();

            _amountLabel.SetWidth(30);
            _expand.OnPressedChanged += pressed => InitListIfFirstRun();
        }

        public void SetValues(Type type, FieldInfo field, List<string> injections)
        {
            _injections = injections;
            TargetType = type;
            TargetField = field;
            if (field == null)
            {
                Debug.Log($"{type} has empty field!");
                return;
            }

            _classLabel = GetDrawer(type.FullName);
            _fieldLabel.Text = field.Name;
            _amountLabel.Text = injections.Count.ToString();
        }

        protected override void AtDraw()
        {
            if (TargetField == null)
                return;

            DTScope.DrawHorizontal(_expand, _classLabel, _fieldLabel, _amountLabel);
            if (_expand.Pressed)
                _listDrawer.Draw();
        }

        private void InitListIfFirstRun()
        {
            if (_isInited)
            {
                return;
            }

            _isInited = true;
            _listDrawer.SetList(_injections, GetDrawer);
        }

        private DTObject<TextAsset> GetDrawer(string fullTypeName) =>
            InjectionCacheUtils.GetDrawer(_csCache, fullTypeName);

        private static string ClearNamespace(string fullName)
        {
            var lastDotPos = fullName.LastIndexOf(".", StringComparison.Ordinal);
            if (lastDotPos < 0)
                return fullName;
            var result = fullName.Substring(lastDotPos + 1);
            return result;
        }
    }
}
