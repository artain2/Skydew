using System;
using UnityEngine;

namespace Jarvis
{
    [Serializable]
    public class JarvisPanelInfo
    {
        public string PanelName;
        public Texture Icon;
        public string Tooltip = "JarvisPanel";
        public bool Enabled = true;
        public TextAsset CodeFile;

        public JarvisPanelInfo(string panelName)
        {
            PanelName = panelName;
        }
    }
}