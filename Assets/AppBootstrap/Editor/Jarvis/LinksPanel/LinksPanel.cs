using System.Collections.Generic;
using System.Linq;
using AppBootstrap.Editor.Jarvis.Listeners;
using AppBootstrap.Editor.Validator;
using AppBootstrap.Runtime.Injector;
using AppBootstrap.Runtime.Utility;
using DrawerTools;
using UnityEditor;

namespace AppBootstrap.Editor.Jarvis.Links
{
    public class LinksPanel : DTPanel
    {
        private List<LinksPanelItemDrawer> _allDrawers = new List<LinksPanelItemDrawer>();
        private InjectorConfig _config;
        private readonly DTString _filterField;
        private readonly List<LinksPanelItemDrawer> _filtered = new List<LinksPanelItemDrawer>();

        public LinksPanel(IDTPanel parent) : base(parent)
        {
            SetExpandable(true);
            _filterField = new DTString("", "");
            _filterField.AddStringChangeCallback(Filter);
        }


        public void SetConfig(InjectorConfig config)
        {
            _config = config;
            var linkInfos = config.InfoList
                .Where(x => x.Links.Any())
                .OrderBy(x => ValidatorUtils.ClearTypeName(x.TypeName))
                .ToArray();
            foreach (var info in linkInfos)
            {
                var serviceName = ValidatorUtils.ClearTypeName(info.TypeName);
                var drawer = new LinksPanelItemDrawer();
                drawer.SetName(serviceName);
                drawer.SetChangeAction(SaveConfig);
                drawer.SetLinks(info.Links);
                _allDrawers.Add(drawer);
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

        private void SaveConfig()
        {
            EditorUtility.SetDirty(_config);
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
                .Where(x => x.ServiceName.ToLower().Contains(filterStr))
                .ToArray();
            var startsWith = allMatches
                .Where(x => x.ServiceName.ToLower().StartsWith(filterStr))
                .ToArray();
            var rest = allMatches
                .Except(startsWith);
            _filtered.AddRange(startsWith);
            _filtered.AddRange(rest);
        }
    }
}