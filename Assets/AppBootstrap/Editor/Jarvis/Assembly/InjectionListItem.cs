using System;
using DrawerTools;
using UnityEngine;

namespace AppBootstrap.Editor.Jarvis.Assembly
{
    public class InjectionListItem : DTDrawable
    {
        public Type TargetType { get; private set; }

        private DTLabel _label;
        private DTObject<TextAsset> _codeFileLisnkField;
        private DTBool _isInAssembly;

        public InjectionListItem(Type type, bool inAssembly)
        {
            TargetType = type;
            
            _label = new DTLabel(type.Name)
            {
                Tooltip = type.FullName
            };
            
            _codeFileLisnkField = new DTObject<TextAsset>("");
            _codeFileLisnkField.SetWidth(160);
            _codeFileLisnkField.Disabled = true;

            _isInAssembly = new DTBool("", inAssembly);
            _isInAssembly.SetWidth(20);
        }

        protected override void AtDraw()
        {
            DTScope.DrawHorizontal(_isInAssembly,_label,_codeFileLisnkField );
        }

        public void SetTextAsset(TextAsset ta)
        {
            _codeFileLisnkField.Value = ta;
        }
    }
}