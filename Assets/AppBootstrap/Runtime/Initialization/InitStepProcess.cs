using System;
using System.Collections.Generic;
using System.Diagnostics;
using AppBootstrap.Runtime.Initialization.InitializationUtils;
using AppBootstrap.Runtime.Utility;

namespace AppBootstrap.Runtime.Initialization
{
    public class InitStepProcess
    {
        private string _stepKey;

        public InitStepProcess(string stepKey)
        {
            _stepKey = stepKey;
        }

        public void Process(List<InitStepItemInfo> infos, Dictionary<string, object> services, Action callback)
        {
            var invokeList = new Dictionary<string, IInvokable>();
            var groupsDict = new Dictionary<string, SerialGroup>();

            foreach (var info in infos)
            {
                var type = BootstrapReflection.GetTypeFromString(info.InjectableTypeName);

                var method = type.GetMethod(info.InitializationMethodName,
                    BootstrapReflection.BindingFlagsNoStatic);

                if (method == null)
                    continue;

                var methodParams = method.GetParameters();
                var hasCallbackParam =
                    methodParams.Length > 0 &&
                    methodParams[0].ParameterType.IsEquivalentTo(typeof(Action));
                if (methodParams.Length > 1 || methodParams.Length == 1 && !hasCallbackParam)
                {
                    continue;
                }


                var target = services[info.InjectableTypeName];

                var invItem = new SerialGroupItem(target, method, hasCallbackParam);

                // Item without group
                if (string.IsNullOrEmpty(info.SerialGroup))
                {
                    invokeList.Add(info.InjectableTypeName, invItem);
                    continue;
                }

                // Item with group
                if (!groupsDict.ContainsKey(info.SerialGroup))
                {
                    var group = new SerialGroup(info.SerialGroup);
                    groupsDict.Add(info.SerialGroup, group);
                    invokeList.Add(info.InjectableTypeName, group);
                }

                groupsDict[info.SerialGroup].Items.Add(invItem);
            }

            // Debug.Log("AllCount = " + invokeList.Count);
            var callbackManager = new StepCallbackManager(invokeList.Count);

            foreach (var invokable in invokeList)
            {
                var processTime = new Stopwatch();
                processTime.Start();
                invokable.Value.CompleteCallback = () =>
                {
                    callbackManager.Callback?.Invoke();
                    var timestep = processTime.Elapsed;
                };
                invokable.Value.Invoke();
            }

            // Everything was instant
            if (callbackManager.IsCompleted)
            {
                callback?.Invoke();
                return;
            }

            // Wait for complete
            callbackManager.OnAllCompleted += () => callback?.Invoke();
        }
    }
}