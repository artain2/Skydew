using System;
using System.Collections;
using System.Collections.Generic;
using DrawerTools;
using UnityEditor;
using UnityEngine;

namespace Jarvis
{
    public class JarvisPanelWindow : DTWindow
    {
        public static Dictionary<string, DTPanel> Panels = new Dictionary<string, DTPanel>();

        public DTPanel Panel;

        private string GetName() => EditorPrefs.GetString($"{nameof(JarvisPanelWindow)}_{DisplayedName}");

        public void SetName(string panelName) =>
            EditorPrefs.SetString($"{nameof(JarvisPanelWindow)}_{DisplayedName}", panelName);

        public static void Show(string panelName)
        {
            var window = DTWindow.Show<JarvisPanelWindow>(panelName);
            window.Panel = window.CreatePanel(panelName);
            window.SetName(panelName);
        }

        protected override void AtDraw()
        {
            Panel?.Draw();
        }

        protected override void AtInit()
        {
            if (Panel != null)
                return;

            var panelName = GetName();
            if (!string.IsNullOrEmpty(panelName))
                Panel = JarvisManager.Instance.CreateInstance(panelName, this);
        }

        private DTPanel CreatePanel(string panelName)
        {
            if (!string.IsNullOrEmpty(panelName))
                JarvisManager.Instance.CreateInstance(panelName, this);
            return null;
        }
    }
}