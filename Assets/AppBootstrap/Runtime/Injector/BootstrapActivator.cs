using System;
using System.Collections.Generic;
using System.Linq;
using AppBootstrap.Runtime.Utility;

namespace AppBootstrap.Runtime.Injector
{
    public class BootstrapActivator
    {
        public IEnumerable<object> CreateInstances(IEnumerable<string> names)
        {
            var types = names.Select(BootstrapReflection.GetTypeFromString);
            var instances = types.Select(Activator.CreateInstance);
            return instances;
        }
    }
}