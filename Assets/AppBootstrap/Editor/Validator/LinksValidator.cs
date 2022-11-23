using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using AppBootstrap.Runtime.Injector.InjectUtils;
using AppBootstrap.Runtime.Utility;
using UnityEngine;

namespace AppBootstrap.Editor.Validator
{
    public static class LinksValidator
    {
        public static void ValidateLinks(List<InjectingInfo> infos)
        {
            foreach (var info in infos)
                ValidateLink(info);
        }

        private static void ValidateLink(InjectingInfo info)
        {
            var type = BootstrapReflection.GetTypeFromString(info.TypeName);
            var injectFields = ValidatorUtils.GetInjectFields(type).ToArray();


            if (!injectFields.Any())
            {
                info.Links.Clear();
                return;
            }

            var linksInjections = injectFields.Where(x =>
                x.FieldType.IsSubclassOf(typeof(Object)) || x.FieldType == typeof(Object)).ToArray();
            
            if (!linksInjections.Any())
            {
                info.Links.Clear();
                return;
            }
            
            
            // Remove obsolete fields
            for (var i = info.Links.Count - 1; i >= 0; i--)
                if (linksInjections.All(x => x.Name != info.Links[i].FieldName))
                    info.Links.RemoveAt(i);

            foreach (var linkField in linksInjections)
            {
                // Try find collection info for this field
                var existingItem =
                    info.Links.FirstOrDefault(
                        x => x.FieldName == linkField.Name);

                if (existingItem != null) 
                    continue;
                
                // New field
                existingItem = new LinksInjectingInfo() 
                {
                    FieldName = linkField.Name
                };
                info.Links.Add(existingItem);

            }
        }
    }
}