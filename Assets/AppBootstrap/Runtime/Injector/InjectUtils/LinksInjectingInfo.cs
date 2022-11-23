using System;
using UnityEngine;

namespace AppBootstrap.Runtime.Injector.InjectUtils
{
    [Serializable]
    public class LinksInjectingInfo
    {
        public string FieldName;
        [SerializeReference] public UnityEngine.Object Value;
    }
}