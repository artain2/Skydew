using System.Collections.Generic;
using System.Linq;
using AppBootstrap.Editor.Validator;
using AppBootstrap.Runtime.ConfigsLoading;
using AppBootstrap.Runtime.Initialization;
using AppBootstrap.Runtime.Initialization.InitializationUtils;
using AppBootstrap.Runtime.Injector;
using DrawerTools;
using UnityEngine;

namespace AppBootstrap.Editor.Jarvis.ConfigsPanel
{
    public class ConfigsPanel : DTPanel
    {
        private DTExpandToggle _expand = new DTExpandToggle();
        private DTObject<BootstrapConfigsLoader> _configLoaderField;
        private DTObject<InjectorConfig> _injectorConfigField;
        private DTObject<InitStepsOrderConfig> _stepsOrderConfigField;
        private List<DTObject<InitStepConfig>> _stepConfigFields;
        private DTButton _validateButton;


        public ConfigsPanel(IDTPanel parent) : base(parent)
        {
            _configLoaderField = new DTObject<BootstrapConfigsLoader>("Root") {Disabled = true};
            _injectorConfigField = new DTObject<InjectorConfig>("Injector") {Disabled = true};
            _stepsOrderConfigField = new DTObject<InitStepsOrderConfig>("Steps") {Disabled = true};
            _stepConfigFields = new List<DTObject<InitStepConfig>>();
            _validateButton = new DTButton("Validate", AtValidate);
            _validateButton.SetWidth(120);
        }

        public void SetConfig(BootstrapConfigsLoader config)
        {
            _configLoaderField.Value = config;
            _injectorConfigField.Value = config.Injector;
            _stepsOrderConfigField.Value = config.StesOrder;
            _stepConfigFields = config.StesOrder.StepConfigs
                .Select(stepConfig => new DTObject<InitStepConfig>(stepConfig.StepKey, stepConfig) {Disabled = true})
                .ToList();
        }


        protected override void AtDraw()
        {
            using (DTScope.Box)
            {
                using (DTScope.Horizontal)
                {
                    _expand.Draw();
                    DrawerTools.DT.Label("Configs", font_style: FontStyle.Bold);
                    _validateButton.Draw();
                }

                if (_expand.Pressed)
                {
                    _configLoaderField.Draw();
                    _injectorConfigField.Draw();
                    _stepsOrderConfigField.Draw();
                    DTScope.BeginHorizontalOffset(inBox: false);
                    foreach (var stepDrawer in _stepConfigFields)
                        stepDrawer.Draw();
                    DTScope.EndHorizontalOffset();
                }
            }
        }

        private void AtValidate()
        {
            InjectValidator.ValidateConfig(_injectorConfigField.Value);
            foreach (var step in _stepConfigFields)
            {
                InitValidator.ValidateStep(step.Value);
            }
        }
    }
}