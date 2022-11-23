using System;
using System.Collections.Generic;
using System.Linq;
using AppBootstrap.Editor.Validator;
using AppBootstrap.Runtime.Injector;
using AppBootstrap.Runtime.Utility;
using DrawerTools;

namespace AppBootstrap.Editor.Jarvis.Realizations
{
    public class RealizationsPanel : DTPanel
    {
        private List<RealizationsDrawer> _allDrawers = new List<RealizationsDrawer>();
        private InjectorConfig _config;
        private DTFilterString<RealizationsDrawer> _filterField;

        public RealizationsPanel(IDTPanel parent) : base(parent)
        {
            SetExpandable(true);
            _filterField = new DTFilterString<RealizationsDrawer>(x => x.TargetType.Name, true);
        }

        protected override void AtDraw()
        {
            _filterField.Draw();
            foreach (var drawer in _allDrawers)
            {
                drawer.Draw();
            }
        }

        public void SetConfig(InjectorConfig config)
        {
            _config = config;
            var infos = config.InfoList
                .Where(x => x.Realizations.Any())
                .OrderBy(x => ClearTypeName(x.TypeName))
                .ToArray();

            var activeTypes = config.InfoList
                .Where(x => x.IsInAssembly)
                .Select(x => ValidatorUtils.GetTypeByString(x.TypeName))
                .ToList();

            foreach (var info in infos)
            {
                var type = ValidatorUtils.GetTypeByString(info.TypeName);
                foreach (var realizationInfo in info.Realizations)
                {
                    Action<string> writeAction = v =>
                    { 
                        realizationInfo.Realization = v;
                        DTAssets.SetDirty(config);
                    };
                    var field = type.GetField(realizationInfo.FieldName,
                        BootstrapReflection.BindingFlagsNoStatic);
                    var drawer = new RealizationsDrawer(this, writeAction);
                    var values = activeTypes
                        .Where(x => field.FieldType.IsAssignableFrom(x))
                        .Union(new[] { field.FieldType })
                        .ToList();
                    drawer.SetValues(type, field, values, realizationInfo.Realization);
                    _allDrawers.Add(drawer);
                }
            }
            _filterField.SetContent(_allDrawers);
        }

        public static string ClearTypeName(string fullTypeName) =>
            string.IsNullOrEmpty(fullTypeName) || !fullTypeName.Contains(".")
                ? fullTypeName
                : fullTypeName.Substring(fullTypeName.LastIndexOf(".") + 1);

    }
}