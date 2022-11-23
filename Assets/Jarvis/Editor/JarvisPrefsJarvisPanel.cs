using System.Linq;
using DrawerTools;


namespace Jarvis
{
    public class JarvisPrefsJarvisPanel : DTPanel
    {
        private JarvisPanelsConfig config;
        private DTButton showConfigButton;
        private DTScriptableObject<JarvisPanelsConfig> configField;

        public JarvisPrefsJarvisPanel(IDTPanel parent) : base(parent)
        {
            config = DTAssets.FindAssetsByType<JarvisPanelsConfig>().First();
            showConfigButton = new DTButton("Config", AtShowConfig);
            configField = new DTScriptableObject<JarvisPanelsConfig>(config);
            configField.SerializeAll();
            panelBehaviour.DrawScrollInExpand = true;
        }

        protected override void AtDraw()
        {
            configField.Draw();
            showConfigButton.Draw();
        }

        private void AtShowConfig()
        {
            DT.ShowAsset(config);
        }
    }
}