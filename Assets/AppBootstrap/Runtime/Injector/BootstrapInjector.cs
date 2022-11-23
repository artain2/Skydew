using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using AppBootstrap.Runtime.Injector.InjectUtils;
using AppBootstrap.Runtime.Utility;
using UnityEngine;
using Object = UnityEngine.Object;

namespace AppBootstrap.Runtime.Injector
{
    public class BootstrapInjector
    {
        private Dictionary<string, object> _instancesDict;

        public void InjectDependencies(IEnumerable<object> instances, IEnumerable<InjectingInfo> infos)
        {
            _instancesDict = instances.ToDictionary(k => k.GetType().FullName);

            foreach (var info in infos)
            {
                InjectByInfo(info);
            }
        }

        private void InjectByInfo(InjectingInfo info)
        {
            if (!info.IsInAssembly)
                return;

            var inst = GetInstanceByFullName(info.TypeName);
            var fieldsToInject = GetInjectFields(inst.GetType());
            var unityObjType = typeof(UnityEngine.Object);
            foreach (var field in fieldsToInject)
            {
                var ft = field.FieldType;

                var isCollection = BootstrapReflection.CheckIsCollectionField(ft, out var elementType);

                if (isCollection)
                {
                    InjectListeners(info, field, elementType, inst);
                    continue;
                }

                var isLink = ft.IsSubclassOf(unityObjType) || ft == unityObjType;
                if (isLink)
                {
                    if (!TryFindLink(info, field, out var linkInst))
                        continue;

                    if (ft.IsSubclassOf(typeof(Component)))
                    {
                        var linkGo = linkInst as GameObject;
                        if (linkGo == null)
                        {
                            Debug.LogError($"[{info.TypeName}] wants MonoBeh, but linked object is not an GameObject");
                            continue;
                        }

                        linkInst = linkGo.GetComponent(ft);
                        if (linkInst == null)
                        {
                            Debug.LogError($"[{info.TypeName}] wants [{ft.Name}], but linked object not contains this component on root");
                            continue;
                        }
                    }

                    field.SetValue(inst, linkInst);
                }

                if (!TryFindRealization(info, field, out var singleFieldInst))
                    continue;

                field.SetValue(inst, singleFieldInst);
                //AppBootstrapper.Logger.Log($"Injeced single: {singleFieldInst.GetType().Name} to {inst.GetType().Name}");
            }
        }

        private bool TryFindRealization(InjectingInfo info, FieldInfo field, out object realization)
        {
            var realizationInfo = info.Realizations?.FirstOrDefault(x => x.FieldName == field.Name);

            if (realizationInfo == null)
                return TryFindInstanceOfType(field.FieldType, out realization);

            var realizationType = BootstrapReflection.GetTypeFromString(realizationInfo.Realization);
            return TryFindInstanceOfType(realizationType, out realization);
        }

        private bool TryFindLink(InjectingInfo info, FieldInfo field, out Object realization)
        {
            realization = null;
            var linkInfo = info.Links?.FirstOrDefault(x => x.FieldName == field.Name);

            if (linkInfo == null)
                return false;

            realization = linkInfo.Value;
            return true;
        }

        private void InjectListeners(InjectingInfo info, FieldInfo field, Type elementType, object inst)
        {
            // Inject collection by rule (order) in info
            var rule = info.CollectionsInjectingOrder.FirstOrDefault(x => x.CollectionFieldName == field.Name);
            if (rule == null)
            {
                Debug.LogError($"Cant find rule for {field.Name}");
                return;
            }

            var toAdd = rule.Injectables.Select(GetInstanceByFullName).ToArray();

            if (field.FieldType.IsArray)
            {
                // Array for array
                var arrayInjection = Array.CreateInstance(elementType, toAdd.Count());
                for (int i = 0; i < toAdd.Length; i++)
                    arrayInjection.SetValue(toAdd[i], i);

                field.SetValue(inst, arrayInjection);
            }
            else
            {
                // List for list and all collection abstractions (IList, IEnumerable, etc)
                var constructedListType = typeof(List<>).MakeGenericType(elementType);
                var collectionInjection = (IList) Activator.CreateInstance(constructedListType);
                foreach (var item in toAdd)
                    collectionInjection.Add(item);
                field.SetValue(inst, collectionInjection);
            }
        }

        private bool TryFindInstanceOfType(Type type, out object inst)
        {
            // Certain field (not an abstraction)
            if (_instancesDict.TryGetValue(type.FullName, out inst))
            {
                return true;
            }

            inst = _instancesDict.Values.FirstOrDefault(type.IsInstanceOfType);
            return inst != null;
        }

        private object GetInstanceByFullName(string name)
        {
            if (_instancesDict.TryGetValue(name, out var result))
            {
                return result;
            }

            Debug.LogError($"Cant find instance of [{name}]. Try validate InjectorConfig");
            return null;
        }

        // private FieldInfo[] GetInjectFields(IReflect type) =>
        //     type.GetFields(BootstrapReflection.BindingFlagsNoStatic)
        //         .Where(x => x.GetCustomAttribute<InjectAttribute>() != null).ToArray();
        private static IEnumerable<FieldInfo> GetInjectFields(Type type)
        {
            var list = new List<FieldInfo>(type.GetFields(BootstrapReflection.BindingFlagsNoStatic));
            if (type.BaseType != null)
            {
                var parentClassFields = type.BaseType.GetFields(BootstrapReflection.BindingFlagsNoStatic);
                // Debug.Log($"{type}\n{type.BaseType}\n{(string.Join(" ", parentClassFields.Select(x => x.Name)))}");
                list.AddRange(parentClassFields);
            }

            var injectFields = list.Where(x => x.GetCustomAttribute<InjectAttribute>() != null);
            return injectFields;
        }
    }
}