using System.Collections.Generic;
using AppBootstrap.Editor.Jarvis.Assembly;
using AppBootstrap.Editor.Jarvis.Links;
using AppBootstrap.Editor.Jarvis.Listeners;
using AppBootstrap.Editor.Jarvis.Realizations;
using AppBootstrap.Runtime.ConfigsLoading;
using DrawerTools;
using UnityEngine;

namespace AppBootstrap.Editor.Jarvis
{
    public class InjectorJarvisPanel : DTPanel
    {
        private ConfigsPanel.ConfigsPanel _configsPanel;
        private DTPanel _activePanel;
        
        private DTToolbar panelToolbar;
        private List<DTPanel> _panels = new List<DTPanel>();

        public InjectorJarvisPanel(IDTPanel parent) : base(parent)
        {
            if (!DTAssets.TryFindAsset<BootstrapConfigsLoader>("DefaultConfigsLoader", "asset", out var config))
            {
                Debug.LogError("Cant find InjectorConfig");
                return;
            }

            _configsPanel = new ConfigsPanel.ConfigsPanel(this);
            _configsPanel.SetConfig(config);
            
            var injPanel = new InjectionListPanel(this);
            injPanel.SetInjections(config.Injector);
            _panels.Add(injPanel);

            var listenersPanel = new ListenersPanel(this);
            listenersPanel.SetConfig(config.Injector);
            _panels.Add(listenersPanel);

            var realizationsPanel = new RealizationsPanel(this);
            realizationsPanel.SetConfig(config.Injector);
            _panels.Add(realizationsPanel);

            var linksPanel = new LinksPanel(this);
            linksPanel.SetConfig(config.Injector);
            _panels.Add(linksPanel);

            panelToolbar = new DTToolbar(
                sender => SetActivePanel(sender.Value), 
                "List", "Collections", "Realizations", "Links");
            _activePanel = _panels[0];
        }

        private void SetActivePanel(int id)
        {
            _activePanel?.SetActive(false);
            _panels[id].SetActive(true);
            _activePanel = _panels[id];
        }

        protected override void AtDraw()
        {
            _configsPanel.Draw();
            panelToolbar.Draw();
            _activePanel.Draw();
        }
    }

    
}