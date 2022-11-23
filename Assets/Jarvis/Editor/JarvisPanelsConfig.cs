using System.Collections.Generic;
using UnityEngine;

namespace Jarvis
{
    public class JarvisPanelsConfig : ScriptableObject
    {
        public List<JarvisPanelInfo> Infos = new List<JarvisPanelInfo>();
        public int SelectedPanelIndex = 0;
        public bool AllowQuotes = false;
        public float SelectPanelWidth = 120f;

        public string SelectedPanelName => Infos[SelectedPanelIndex].PanelName;

        public JarvisPanelInfo this[int i] => Infos[i];
    }
}