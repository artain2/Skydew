using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using DrawerTools;
using UnityEngine;

namespace Jarvis
{
    public class JarvisEditorWindow : DTWindow
    {
        private SelectPanel _selector;

        // private DTTogglePanel _header;
        private Dictionary<string, DTPanel> _handleDict;
        private DTPanel _activePanel;
        private JarvisPanelsConfig _config;

        private bool _inited = false;

        private string SelectedPanelName
        {
            get => EditorPrefs.GetString($"{nameof(JarvisEditorWindow)}_{nameof(SelectedPanelName)}", DefaultPanelName);
            set => EditorPrefs.SetString($"{nameof(JarvisEditorWindow)}_{nameof(SelectedPanelName)}", value);
        }

        #region DT

        [MenuItem("Editors/Jarvis %#J")]
        public static void ShowWindow() => Show<JarvisEditorWindow>();

        public override string WindowName => "Jarvis";

        private const string DefaultPanelName = "JarvisPrefsJarvisPanel";

        protected override void AtInit()
        {
            ManageHandles();

            foreach (var kvp in _handleDict)
                kvp.Value.SetActive(false);


            var enabledInfos = _config.Infos.Where(x => x.Enabled).ToArray();
            _selector = new SelectPanel(this);
            _selector
                .SetItems(enabledInfos.Select(x => x.Tooltip).ToList())
                .SetClickAction(AtPanelButtonClick)
                .SetItemsWidth(_config.SelectPanelWidth - 10f)
                .SetWidth(_config.SelectPanelWidth);

            _activePanel = GetPanel(SelectedPanelName);
            if (_activePanel == null)
            {
                SelectedPanelName = DefaultPanelName;
                _activePanel = GetPanel(SelectedPanelName);
            }
            _activePanel.SetActive(true);
            SetSelectorButtonActive(SelectedPanelName);
            
            wantsMouseMove = true;
            if (_config.AllowQuotes)
                Debug.Log(JarvisQuotes.GetRandomQuote());
            _inited = true;
        }

        protected override void AtDraw()
        {
            using (DTScope.Horizontal)
            {
                using (DTScope.Vertical)
                {
                    _selector.Draw();
                }

                using (DTScope.Vertical)
                {
                    _activePanel.Draw();
                }
            }
        }

        public DTPanel GetPanel(string panelName)
        {
            if (_handleDict.TryGetValue(panelName, out var panel))
                return panel;

            var pName = JarvisManager.Instance.AllPanelTypes.FirstOrDefault(x => x.Name == panelName);
            if (pName == null)
                return null;
            panel = _handleDict[panelName] = JarvisManager.Instance.CreateInstance(pName.Name, this);
            panel.SetName(panelName);
            return panel;
        }

        #endregion DT

        private void ManageHandles()
        {
            _config = GetOrCreateConfig();
            _handleDict = new Dictionary<string, DTPanel>();

            var allPanelTyped = JarvisManager.Instance.AllPanelTypes;

            // Remove not actual
            var removeDict = _config.Infos.ToDictionary(x => x.PanelName);
            foreach (var type in allPanelTyped)
                if (removeDict.ContainsKey(type.Name))
                    removeDict.Remove(type.Name);
            foreach (var kvp in removeDict)
                _config.Infos.Remove(kvp.Value);

            // Add new and set handles
            foreach (var type in allPanelTyped)
            {
                var info = _config.Infos.FirstOrDefault(x => type.Name == x.PanelName);
                if (info != null)
                {
                    if (info.CodeFile == null)
                        info.CodeFile = info.CodeFile = GetCodeFile(type.Name);
                    continue;
                }

                info = new JarvisPanelInfo(type.Name);
                info.CodeFile = GetCodeFile(type.Name);
                _config.Infos.Add(info);
            }

            DTAssets.SetDirty(_config);

            TextAsset GetCodeFile(string className)
            {
                DTAssets.TryFindClassAsset(className, out var codeFile);
                return codeFile;
            }
        }


        private void AtPanelButtonClick(int id)
        {
            if (DTKeys.IsRightMouse)
            {
                DTGenericMenu.New()
                    .AddItem("Open", () => SwitchPanel(id))
                    .AddItem("Open in new window", () => OpenInWindow(id))
                    .AddItem("Refresh", () => RefreshPanel(id))
                    .AddItem("Show code file", () => ShowCode(id))
                    .Show();
                return;
            }


            SwitchPanel(id);
            SetSelectorButtonActive(SelectedPanelName);
        }

        private void SetSelectorButtonActive(string panelName)
        {
            var btnName = _config.Infos.
                FirstOrDefault(x => x.PanelName == SelectedPanelName)?.Tooltip;
            _selector.SelectButton(btnName);
        }

        private void RefreshPanel(int panelId)
        {
            var panelName = _config.Infos[panelId].PanelName;
            _handleDict[panelName] = JarvisManager.Instance.CreateInstance(panelName, this);
            SwitchPanel(panelId);
        }

        private void ShowCode(int panelId) => DT.ShowAsset(_config.Infos[panelId].CodeFile);

        private void SwitchPanel(int id)
        {
            var panelName = _config.Infos[id].PanelName;
            _activePanel = GetPanel(panelName);
            _activePanel.Active = true;
            SelectedPanelName = panelName;
            DTAssets.SetDirty(_config);
        }

        private void OpenInWindow(int id)
        {
            var panelName = _config.Infos[id].PanelName;
            JarvisPanelWindow.Show(panelName);
        }

        private JarvisPanelsConfig GetOrCreateConfig()
        {
            const string configName = "JarvisPanelsConfig";
            var configFound = DTAssets.TryFindAsset<JarvisPanelsConfig>
                (configName, "asset", out var config, "Editor/");

            if (configFound)
                return config;

            DTAssets.TryFindClassAsset(GetType(), out var ownCsAsset);
            var path = AssetDatabase.GetAssetPath(ownCsAsset);
            var directory = Path.GetDirectoryName(path);
            directory += "/_Content";
            if (!Directory.Exists(directory))
                Directory.CreateDirectory(directory);

            config = ScriptableObject.CreateInstance<JarvisPanelsConfig>();
            DTAssets.CreateConfig(directory, configName, config);
            DT.ShowAsset(config);

            return config;
        }
    }
}