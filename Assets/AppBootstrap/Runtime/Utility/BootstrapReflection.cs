using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace AppBootstrap.Runtime.Utility
{
    public static class BootstrapReflection
    {
        public static IEnumerable<Type> GetInjectablesTypeList() =>
            typeof(InjectableAttribute).Assembly.GetTypes()
                .Where(x => !x.IsAbstract)
                .Where(x => !x.IsGenericTypeDefinition)
                .Where(x => x.GetCustomAttribute<InjectableAttribute>() != null);

        public static IEnumerable<Type> GetAllChildTypes(Type parent)
        {
            var types = AppDomain.CurrentDomain.GetAssemblies()
                .SelectMany(s => s.GetTypes())
                .Where(p => parent.IsAssignableFrom(p));
            return types;
        }

        public static Type GetTypeFromString(string stringType) =>
            typeof(InjectableAttribute).Assembly.GetType(stringType);


        public static BindingFlags BindingFlagsNoStatic =>
            BindingFlags.Public | BindingFlags.NonPublic |
            BindingFlags.Instance | BindingFlags.DeclaredOnly;

        public static bool CheckIsCollectionField(Type fieldType, out Type itemType)
        {
            if (fieldType.IsArray)
            {
                itemType = fieldType.GetElementType();
                return true;
            }

            if (fieldType.IsGenericType && (typeof(IEnumerable).IsAssignableFrom(fieldType)))
            {
                itemType = fieldType.GenericTypeArguments[0];
                return true;
            }

            itemType = null;
            return false;
        }
    }
}