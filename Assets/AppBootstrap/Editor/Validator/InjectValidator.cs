using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using AppBootstrap.Runtime;
using AppBootstrap.Runtime.Injector;
using AppBootstrap.Runtime.Injector.InjectUtils;
using AppBootstrap.Runtime.Utility;

namespace AppBootstrap.Editor.Validator
{
    public static class InjectValidator
    {
        public static void ValidateConfig(InjectorConfig config)
        {
            ValidateCollection(config.InfoList);
            LinksValidator.ValidateLinks(config.InfoList);
#if UNITY_EDITOR
            UnityEditor.EditorUtility.SetDirty(config);
#endif
        }

        private static void ValidateCollection(List<InjectingInfo> infos)
        {
            var allInjectables = BootstrapReflection.GetInjectablesTypeList().ToArray();
            // Remove not actual entrances
            for (int i = infos.Count - 1; i >= 0; i--)
            {
                if (allInjectables.Any(x => x.FullName == infos[i].TypeName))
                    continue;
                infos.RemoveAt(i);
            }

            // Add new entrances
            foreach (var inj in allInjectables)
            {
                if (infos.Any(x => x.TypeName == inj.FullName))
                    continue;
                var info = new InjectingInfo() {TypeName = inj.FullName, IsInAssembly = true};
                infos.Add(info);
            }

            // Separate active/inactive
            var onlyInactiveTypeNames = new List<string>();
            var onlyActiveTypes = new List<Type>();
            foreach (var info in infos)
            {
                var type = BootstrapReflection.GetTypeFromString(info.TypeName);
                if (info.IsInAssembly)
                {
                    onlyActiveTypes.Add(type);
                }
                else
                {
                    onlyInactiveTypeNames.Add(type.FullName);
                }
            }

            ValidateListeners(infos, onlyActiveTypes);
            ValidateRealizations(infos, onlyActiveTypes);
        }

        private static void ValidateRealizations(List<InjectingInfo> infos, List<Type> activeTypes)
        {
            var subtypes = new Dictionary<Type, List<Type>>();

            foreach (var info in infos)
            {
                //info.Realizations.Clear();
                var type = BootstrapReflection.GetTypeFromString(info.TypeName);
                var injectFields = ValidatorUtils.GetInjectFields(type).ToArray();


                if (!injectFields.Any())
                {
                    info.Realizations.Clear();
                    continue;
                }


                var singleInjections =
                    injectFields.Where(x => !BootstrapReflection.CheckIsCollectionField(x.FieldType, out _));
                var singleFields = singleInjections as FieldInfo[] ?? singleInjections.ToArray();

                // Remove obsolete fields
                for (var i = info.Realizations.Count - 1; i >= 0; i--)
                    if (singleFields.All(x => x.Name != info.Realizations[i].FieldName))
                        info.Realizations.RemoveAt(i);

                var realizationsList = info.Realizations;
                foreach (var field in singleFields)
                {
                    if (BootstrapReflection.CheckIsCollectionField(field.FieldType, out var targetType))
                        continue;

                    var moreThenOne = MoreThenOneRealization(field.FieldType);
                    var realiztionInfo = realizationsList.FirstOrDefault(x => x.FieldName == field.Name);
                    var isInInfo = realiztionInfo != null;

                    if (isInInfo && moreThenOne || !isInInfo && !moreThenOne) // Its valid
                    {
                        continue;
                    }

                    if (isInInfo && !moreThenOne) // It has only 1 realization. Remove Info from config
                    {
                        realizationsList.Remove(realiztionInfo);
                        continue;
                    }

                    realiztionInfo = new RealizationInjectingInfo()
                        {FieldName = field.Name, Realization = field.FieldType.FullName};
                    realizationsList.Add(realiztionInfo);
                }
            }

            bool MoreThenOneRealization(Type type)
            {
                if (!subtypes.TryGetValue(type, out var realizations))
                {
                    realizations = activeTypes.Where(x => type.IsAssignableFrom(x)).ToList();
                    subtypes.Add(type, realizations);
                }

                return realizations.Count > 1;
            }
        }

        private static void ValidateListeners(List<InjectingInfo> infos, List<Type> activeTypes)
        {
            // Validate collections
            foreach (var info in infos)
            {
                var type = BootstrapReflection.GetTypeFromString(info.TypeName);
                var injectFields = ValidatorUtils.GetInjectFields(type).ToArray();


                if (!injectFields.Any())
                {
                    info.CollectionsInjectingOrder.Clear();
                    continue;
                }

                var collectionInjections = injectFields.Where(x =>
                    x.FieldType.IsArray ||
                    x.FieldType.GetInterface(nameof(IEnumerable)) != null);
                var collectionFields = collectionInjections as FieldInfo[] ?? collectionInjections.ToArray();

                // Remove obsolete fields
                for (var i = info.CollectionsInjectingOrder.Count - 1; i >= 0; i--)
                    if (collectionFields.All(x => x.Name != info.CollectionsInjectingOrder[i].CollectionFieldName))
                        info.CollectionsInjectingOrder.RemoveAt(i);

                foreach (var collectionField in collectionFields)
                {
                    if (!BootstrapReflection.CheckIsCollectionField(collectionField.FieldType, out var targetType))
                    {
                        // its not a collection, lol
                        continue;
                    }

                    // Suitable services to fill this collection
                    var suitTypes = activeTypes.Where(x => targetType.IsAssignableFrom(x));
                    var suitTypeNames = suitTypes.Select(x => x.FullName).ToList();

                    // Try find collection info for this field
                    var existingItem =
                        info.CollectionsInjectingOrder.FirstOrDefault(
                            x => x.CollectionFieldName == collectionField.Name);

                    // New field without order
                    if (existingItem == null)
                    {
                        existingItem = new CollectionInjectingOrder
                        {
                            TargetType = targetType.FullName,
                            CollectionFieldName = collectionField.Name,
                            Injectables = suitTypeNames
                        };
                        info.CollectionsInjectingOrder.Add(existingItem);
                        continue;
                    }

                    var inectList = new List<string>();
                    for (int i = 0; i < existingItem.Injectables.Count; i++)
                    {
                        // Same same
                        if (suitTypeNames.Contains(existingItem.Injectables[i]))
                        {
                            inectList.Add(existingItem.Injectables[i]);
                        }

                        // Namespace changed
                        var existItemName = existingItem.Injectables[i].Split('.').Last();
                        var suitableSameName = suitTypes.FirstOrDefault(x => x.Name == existItemName);
                        if (suitableSameName != null)
                        {
                            inectList.Add(suitableSameName.FullName);
                        }
                    }

                    // Add new types (if they don't registered yet)
                    existingItem.Injectables = inectList.Union(suitTypeNames).ToList();
                }
            }
        }
    }
}