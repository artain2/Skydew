using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using AppBootstrap.Editor.Jarvis.Utils;
using AppBootstrap.Editor.Validator;
using DrawerTools;
using UnityEngine;

namespace AppBootstrap.Editor.Jarvis.Realizations
{
    public class RealizationsDrawer : DTPanel
    {
        public Type TargetType { get; private set; }
        public FieldInfo TargetField { get; private set; }
        public Type SelectedType { get; private set; }

        private DTObject<TextAsset> _classDrawer = new DTObject<TextAsset>("");
        private DTLabel _fieldLabel = new DTLabel("");
        private DTPopup _valueSelector = new DTPopup();
        private InjectionFilesCache _csCache;
        private Dictionary<string, Type> _typesDict = new Dictionary<string, Type>();
        private Action<string> _writeAction;


        public RealizationsDrawer(IDTPanel parent, Action<string> writeAction) : base(parent)
        {
            _writeAction = writeAction;
            _csCache = InjectionCacheUtils.GetConfig();
            _valueSelector.OnPopupValueChanged += AtValueSelected;
        }



        protected override void AtDraw()
        {
            using (DTScope.Horizontal)
            {
                _classDrawer.Draw();
                _fieldLabel.Draw();
                _valueSelector.Draw();
            }
        }

        public void SetValues(Type type, FieldInfo field, List<Type> allownTypes, string selectedValue)
        {
            TargetType = type;
            TargetField = field;
            if (field == null)
            {
                Debug.Log($"{type} has empty field!");
                return;
            }
            _classDrawer = InjectionCacheUtils.GetDrawer(_csCache, type.FullName);
            _fieldLabel.Text = field.Name;
            var values = allownTypes
                .Select(x => x.Name)
                .ToArray();
            _typesDict = allownTypes.ToDictionary(k => k.Name, v => v);
            var selected = ValidatorUtils.ClearTypeName(selectedValue);
            if (allownTypes.All(x=>x.FullName!= selectedValue))
            {
                Debug.LogError($"Invalid selected value ({selectedValue})");
                return;
            }
            _valueSelector.SetValues(values, selected);
            SetSelectedType(selected);
        }

        private void ValidateLabelColor()
        {
            var isValid = !SelectedType.IsAbstract && !SelectedType.IsInterface;
            _fieldLabel.SetColor(isValid ? DTLabel.DefaultColor : Color.red);
        }

        private void AtValueSelected(DTPopup sender)
        {
            SetSelectedType(sender.ActiveValue);
        }

        private void SetSelectedType(string typeName)
        {
            var t = _typesDict[typeName];
            SelectedType = t;
            ValidateLabelColor();
            _writeAction(t.FullName);
        }
    }
}