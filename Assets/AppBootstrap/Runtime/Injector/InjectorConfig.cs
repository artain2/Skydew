using System.Collections.Generic;
using AppBootstrap.Runtime.Injector.InjectUtils;
using UnityEngine;

namespace AppBootstrap.Runtime.Injector
{
    /// <summary>
    /// Конфиг содержащий список всех используемых сервисов в приложении 
    /// </summary>
    [CreateAssetMenu(fileName = nameof(InjectorConfig), menuName = "Configs/Bootstrap/" + nameof(InjectorConfig))]
    public class InjectorConfig : ScriptableObject
    {
        public List<InjectingInfo> InfoList = new List<InjectingInfo>();
    }
}