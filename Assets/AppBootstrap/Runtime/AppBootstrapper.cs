using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using AppBootstrap.Runtime.ConfigsLoading;
using AppBootstrap.Runtime.Initialization;
using AppBootstrap.Runtime.Injector;
using UnityEngine;
using Debug = UnityEngine.Debug;

namespace AppBootstrap.Runtime
{
    public class AppBootstrapper : MonoBehaviour
    {
        
        public Action OnInitialized;

        public BootstrapConfigsLoader _configLoader;

        private Stopwatch _processTime;

        private void Awake()
        {
            if (ServiceLocator.Current != null)
            {
                Destroy(gameObject);
                return;
            }

            _processTime = new Stopwatch();
            _processTime.Start();
            DontDestroyOnLoad(gameObject);
            _configLoader.LoadConfigs(AtConfigsLoaded);
        }

        private void AtConfigsLoaded(InjectorConfig injectorConfig, InitStepsOrderConfig stepsOrderConfig)
        {
            ServiceLocator.Initiailze();
            var instances = ActivateInstances(injectorConfig).ToArray();
            RegisterService(instances);
            InjectInstances(injectorConfig, instances);
            InitServices(stepsOrderConfig, instances);
        }

        private IEnumerable<object> ActivateInstances(InjectorConfig config)
        {
            var activator = new BootstrapActivator();
            var toActivateNames = config.InfoList.Where(x => x.IsInAssembly).Select(x => x.TypeName);
            var instances = activator.CreateInstances(toActivateNames);
            return instances;
        }

        private void InjectInstances(InjectorConfig config, IEnumerable<object> instances)
        {
            var injector = new BootstrapInjector();
            injector.InjectDependencies(instances, config.InfoList);
        }

        private void RegisterService(IEnumerable<object> instances)
        {
            instances.ToList().ForEach(x => ServiceLocator.Current.Register(x));
        }

        private void InitServices(InitStepsOrderConfig config, IEnumerable<object> instances)
        {
            var initer = new InitProcess(config, instances, AtInitCompleted);
            initer.Process();
        }

        private void AtInitCompleted()
        {
            _processTime.Stop();
            Debug.Log($"All services initialized! [{_processTime.Elapsed}]");
            ServiceLocator.IsInited = true;
            ServiceLocator.OnInited?.Invoke();
            OnInitialized?.Invoke();
        }
    }
}