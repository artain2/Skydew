using System.Linq;
using AppBootstrap.Editor.Validator;
using DrawerTools;
using UnityEngine;

namespace AppBootstrap.Editor.Jarvis.Utils
{
    public static class InjectionCacheUtils
    {
        public static InjectionFilesCache GetConfig()
        {
            DTAssets.TryFindAsset<InjectionFilesCache>("InjectionFilesCache", "asset", out var csCache);
            return csCache;
        }

        public static void ValidateConfig()
        {
            var injTypes = ValidatorUtils.GetInjectablesTypeList();
            var csCache = GetConfig();
            foreach (var item in injTypes)
            {
                var node = csCache.Nodes.FirstOrDefault(x => x.FullName == item.FullName);
                if (node!= null && node.CodeFile!=null)
                {
                    continue;
                }
                DTAssets.TryFindClassAsset(item, out var csAsset);
                if (node == null)
                {
                    node = new InjectionFilesCache.Node() {FullName = item.FullName};
                    csCache.Nodes.Add(node);
                }

                node.CodeFile = csAsset;
            }
            DTAssets.SetDirty(csCache);
        }

        public static DTObject<TextAsset> GetDrawer(InjectionFilesCache csCache, string typeName)
        {
            var node = csCache.Nodes.FirstOrDefault(x => x.FullName == typeName);
            if (node == null || node.CodeFile == null)
            {
                return new DTObject<TextAsset>(typeName) {Disabled = true};
            }

            var result = new DTObject<TextAsset>("", node.CodeFile) {Disabled = true};
            return result;
        }
    }
}