using System.Collections.Generic;
using System.Linq;
using AppBootstrap.Editor.Validator;
using AppBootstrap.Runtime.Injector;
using AppBootstrap.Runtime.Utility;
using DrawerTools;

namespace AppBootstrap.Editor.Jarvis.Listeners
{
    public class ListenersPanel : DTPanel
    {
        private List<ListenersDrawer> _allDrawers = new List<ListenersDrawer>();
        private InjectorConfig _config;
        private readonly DTString _filterField;
        private readonly List<ListenersDrawer> _filtered = new List<ListenersDrawer>();

        public ListenersPanel(IDTPanel parent) : base(parent)
        {
            SetExpandable(true);
            _filterField = new DTString("", "");
            _filterField.AddStringChangeCallback(Filter);
        }


        public void SetConfig(InjectorConfig config)
        {
            _config = config;
            var collectionInfos = config.InfoList
                .Where(x => x.CollectionsInjectingOrder.Any())
                .OrderBy(x => ValidatorUtils.ClearTypeName(x.TypeName))
                .ToArray();
            foreach (var info in collectionInfos)
            {
                var type = ValidatorUtils.GetTypeByString(info.TypeName);
                foreach (var collectionInjInfo in info.CollectionsInjectingOrder)
                {
                    var field = type.GetField(collectionInjInfo.CollectionFieldName,
                        BootstrapReflection.BindingFlagsNoStatic);
                    var drawer = new ListenersDrawer(this);
                    drawer.SetValues(type, field, collectionInjInfo.Injectables);
                    _allDrawers.Add(drawer);
                }
            }

            Filter("");
        }

        protected override void AtDraw()
        {
            _filterField.Draw();
            foreach (var drawer in _filtered)
            {
                drawer.Draw();
            }
        }

        private void Filter(string filterStr)
        {
            filterStr = filterStr.ToLower();
            _filtered.Clear();
            if (string.IsNullOrEmpty(filterStr))
            {
                _filtered.AddRange(_allDrawers);
                return;
            }

            var allMatches = _allDrawers
                .Where(x => x.TargetType.Name.ToLower().Contains(filterStr))
                .ToArray();
            var startsWith = allMatches
                .Where(x => x.TargetType.Name.ToLower().StartsWith(filterStr))
                .ToArray();
            var rest = allMatches
                .Except(startsWith);
            _filtered.AddRange(startsWith);
            _filtered.AddRange(rest);
        }
    }
}