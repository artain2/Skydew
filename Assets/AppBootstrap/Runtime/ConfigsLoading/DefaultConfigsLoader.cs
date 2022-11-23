using System;
using AppBootstrap.Runtime.Initialization;
using AppBootstrap.Runtime.Injector;
using UnityEngine;

namespace AppBootstrap.Runtime.ConfigsLoading
{
    [CreateAssetMenu(fileName = nameof(DefaultConfigsLoader), menuName = "Configs/Bootstrap/" + nameof(DefaultConfigsLoader))]
    public class DefaultConfigsLoader : BootstrapConfigsLoader
    {
        [SerializeField] private InjectorConfig injectorConfig;
        [SerializeField] private InitStepsOrderConfig stepsConfig;

        public override InjectorConfig Injector => injectorConfig;
        public override InitStepsOrderConfig StesOrder => stepsConfig;

        public override void LoadConfigs(Action<InjectorConfig, InitStepsOrderConfig> loadCompleteCallback)
        {
            loadCompleteCallback?.Invoke(injectorConfig, stepsConfig);
        }
    }
}