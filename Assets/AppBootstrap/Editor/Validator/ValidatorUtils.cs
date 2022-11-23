using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using AppBootstrap.Runtime;
using AppBootstrap.Runtime.Utility;

namespace AppBootstrap.Editor.Validator
{
    public static class ValidatorUtils
    {
        
        public static IEnumerable<FieldInfo> GetInjectFields(Type type)
        {
            var list = new List<FieldInfo>(type.GetFields(BootstrapReflection.BindingFlagsNoStatic));
            if (type.BaseType != null)
            {
                var parentClassFields = type.BaseType.GetFields(BootstrapReflection.BindingFlagsNoStatic);
                // Debug.Log($"{type}\n{type.BaseType}\n{(string.Join(" ", parentClassFields.Select(x => x.Name)))}");
                list.AddRange(parentClassFields);
            }

            // var injectFields = fields.Where(x => x.GetCustomAttribute<InjectAttribute>() != null);
            var injectFields = list.Where(x => x.GetCustomAttribute<InjectAttribute>() != null);
            return injectFields;
        }
        
        public static IEnumerable<Type> GetInjectablesTypeList() =>
            typeof(InjectableAttribute).Assembly.GetTypes()
                .Where(x => !x.IsAbstract)
                .Where(x => !x.IsGenericTypeDefinition)
                .Where(x => x.GetCustomAttribute<InjectableAttribute>() != null);

        public static Type GetTypeByString(string typeName) =>
            typeof(InjectableAttribute).Assembly.GetType(typeName);

        public static IEnumerable<FieldInfo> GetInjectableFields(Type type)
        {
            var fields = type.GetFields(Flags);
            var injectFields = fields.Where(x => x.GetCustomAttribute<InjectAttribute>() != null);
            return injectFields;
        }
        
        public static bool IsCollectionField(Type fieldType)
            => GetElementType(fieldType) != null;

        public static string ClearTypeName(string fullTypeName) =>
            string.IsNullOrEmpty(fullTypeName) || !fullTypeName.Contains(".")
                ? fullTypeName
                : fullTypeName.Substring(fullTypeName.LastIndexOf(".") + 1);

        public static Type GetElementType(Type fieldType)
        {
            if (fieldType.IsArray)
                return fieldType.GetElementType();

            if (fieldType.IsGenericType && (typeof(IEnumerable).IsAssignableFrom(fieldType)))
                return fieldType.GenericTypeArguments[0];

            return null;
        }

        public static BindingFlags Flags =>
            BindingFlags.Public | BindingFlags.NonPublic |
            BindingFlags.Instance | BindingFlags.DeclaredOnly;
    }
}