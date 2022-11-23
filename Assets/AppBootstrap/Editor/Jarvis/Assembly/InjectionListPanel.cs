using System.Collections.Generic;
using System.Linq;
using AppBootstrap.Editor.Jarvis.Utils;
using AppBootstrap.Runtime.Injector;
using AppBootstrap.Runtime.Utility;
using DrawerTools;
using UnityEngine;

namespace AppBootstrap.Editor.Jarvis.Assembly
{
    public class InjectionListPanel : DTPanel
    {
        private List<InjectionListItem> _allItems = new List<InjectionListItem>();
        private DTButton _findCodeFilesButton;
        private readonly DTString _filterField;
        private readonly List<InjectionListItem> _filtered = new List<InjectionListItem>();
        

        public InjectionListPanel(IDTPanel parent) : base(parent)
        {

            _filterField = new DTString("", "");
            _filterField.AddStringChangeCallback(Filter);
            
            SetExpandable(true);
            _findCodeFilesButton = new DTButton("Find Code Files", ValidateCodeFiles).SetWidth(100) as DTButton;
        }

        public void SetInjections(InjectorConfig config)
        {
            _allItems.Clear();
            foreach (var info in config.InfoList)
            {
                var type = BootstrapReflection.GetTypeFromString(info.TypeName);
                
                if (type == null)
                {
                    Debug.LogError($"cant find type {info.TypeName}");
                    continue;
                }
                var item = new InjectionListItem(type, info.IsInAssembly);
                _allItems.Add(item);
            }

            _allItems = _allItems.OrderBy(x => x.TargetType.Name).ToList();
            ValidateCodeFiles();
            Filter("");
        }

        private void ValidateCodeFiles()
        {
            // var stopwatch = new Stopwatch();
            // stopwatch.Start();

            DTAssets.TryFindAsset<InjectionFilesCache>("InjectionFilesCache", "asset", out var csCache);
            foreach (var item in _allItems)
            {
                var node = csCache.Nodes.FirstOrDefault(x => x.FullName == item.TargetType.FullName);
                if (node!= null && node.CodeFile!=null)
                {
                    item.SetTextAsset(node.CodeFile);
                    continue;
                }
                DTAssets.TryFindClassAsset(item.TargetType, out var csAsset);
                item.SetTextAsset(csAsset);
                if (node == null)
                {
                    node = new InjectionFilesCache.Node() {FullName = item.TargetType.FullName};
                    csCache.Nodes.Add(node);
                }

                node.CodeFile = csAsset;
            }
            DTAssets.SetDirty(csCache);

            // stopwatch.Stop();
            // Debug.Log(Math.Round(stopwatch.Elapsed.TotalSeconds, 2));
        }

        private void Filter(string filterStr)
        {
            filterStr = filterStr.ToLower();
            _filtered.Clear();
            if (string.IsNullOrEmpty(filterStr))
            {
                _filtered.AddRange(_allItems);
                return;
            }
            var allMatches = _allItems
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

        protected override void AtDraw()
        {
            // _findCodeFilesButton.Draw();
            _filterField.Draw();
            using (DTScope.Box)
            {
                foreach (var item in _filtered)
                {
                    item.Draw();
                }
            }
        }
    }
}