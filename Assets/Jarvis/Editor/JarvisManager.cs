using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using DrawerTools;
using UnityEngine;

namespace Jarvis
{
    public class JarvisManager
    {
        private static JarvisManager _instance;
        public static JarvisManager Instance => _instance ??= new JarvisManager();
        
        private Type[] _allPanelTypes;
        public Type[] AllPanelTypes => _allPanelTypes;

        public JarvisManager()
        {
            _allPanelTypes = GetAllPanelTypes();
        }

      public new DTPanel CreateInstance(string panelName, IDTPanel window)
            => (DTPanel) Activator.CreateInstance(_allPanelTypes.FirstOrDefault(x => x.Name == panelName), window);

        private static Type[] GetAllPanelTypes()
        {
            var jarvisPanelTypes = AppDomain.CurrentDomain.GetAssemblies()
                .SelectMany(s => s.GetTypes())
                .Where(x => typeof(DTPanel).IsAssignableFrom(x))
                .Where(x => x.Name.EndsWith("JarvisPanel"))
                .ToArray();

            return jarvisPanelTypes;
        }
    }
}