using System;
using AppBootstrap.Runtime.Initialization;
using AppBootstrap.Runtime.Injector;
using UnityEngine;

namespace AppBootstrap.Runtime.ConfigsLoading
{
    public abstract class BootstrapConfigsLoader : ScriptableObject
    {
        public abstract InjectorConfig Injector { get; }
        public abstract InitStepsOrderConfig StesOrder { get; }
        public abstract void LoadConfigs(Action<InjectorConfig, InitStepsOrderConfig> loadCompleteCallback);
    }
}