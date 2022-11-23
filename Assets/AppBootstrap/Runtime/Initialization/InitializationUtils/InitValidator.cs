using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using static AppBootstrap.Runtime.Utility.BootstrapReflection;

namespace AppBootstrap.Runtime.Initialization.InitializationUtils
{
    public static class InitValidator
    {
        public static void ValidateAllSteps()
        {
        }

        public static void ValidateStep(InitStepConfig stepConfig)
        {
            var stepKey = stepConfig.StepKey;
            var infos = stepConfig.InfoList;
            var allInjectableTypes = GetInjectablesTypeList().ToArray();
            var initInThisStepTypes = new Dictionary<string, Tuple<Type, IEnumerable<MethodInfo>>>();
            foreach (var injType in allInjectableTypes)
            {
                if (!TryGetStepMethods(injType, stepKey, out var methods))
                    continue;
                initInThisStepTypes.Add(
                    injType.FullName,
                    new Tuple<Type, IEnumerable<MethodInfo>>(injType, methods));
            }

            // Remove not actual injectables
            infos.RemoveAll(x => !initInThisStepTypes.ContainsKey(x.InjectableTypeName));

            // Check methods
            foreach (var inj in initInThisStepTypes)
            {
                var injTypeRegisteredMethods =
                    infos.Where(x => x.InjectableTypeName == inj.Key).ToArray();

                var initTypeActualMethods = inj.Value.Item2.ToArray();
                // Remove not actual methods
                foreach (var registered in injTypeRegisteredMethods)
                {
                    if (initTypeActualMethods.All(x => x.Name != registered.InitializationMethodName))
                    {
                        infos.Remove(registered);
                    }
                }

                // Add new methods
                infos.AddRange(
                    from actual in initTypeActualMethods
                    where injTypeRegisteredMethods.All(x => x.InitializationMethodName != actual.Name)
                    select new InitStepItemInfo()
                        {InjectableTypeName = inj.Key, InitializationMethodName = actual.Name});
            }

#if UNITY_EDITOR
            UnityEditor.EditorUtility.SetDirty(stepConfig);
#endif
        }

        private static bool TryGetStepMethods(IReflect type, string stepKey, out IEnumerable<MethodInfo> result)
        {
            var allMethods = type.GetMethods(BindingFlagsNoStatic);
            result = (
                from method in allMethods
                let stepAttribute = method.GetCustomAttribute<InitAttribute>()
                where stepAttribute != null && stepAttribute.Name == stepKey
                select method);
            return result.Any();
        }
    }
}