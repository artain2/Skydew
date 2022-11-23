using System;
using System.Collections.Generic;

namespace AppBootstrap.Runtime.Injector.InjectUtils
{
    /// <summary>
    /// Info about order of injecting in single (collection) field in single injectable
    /// </summary>
    [Serializable]
    public class CollectionInjectingOrder
    {
        public string CollectionFieldName;
        public string TargetType;
        public List<string> Injectables;
    }
}