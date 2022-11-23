using System.Collections.Generic;
using AppBootstrap.Runtime.Initialization.InitializationUtils;
using UnityEngine;
#if ODIN_INSPECTOR && UNITY_EDITOR
using Sirenix.OdinInspector;
#endif

namespace AppBootstrap.Runtime.Initialization
{
    [CreateAssetMenu(fileName = nameof(InitStepConfig), menuName = "Configs/Bootstrap/" + nameof(InitStepConfig))]
    public class InitStepConfig : ScriptableObject
    {
        public bool IsEnabled;
        public string StepKey;
        public List<InitStepItemInfo> InfoList = new List<InitStepItemInfo>();

#if ODIN_INSPECTOR && UNITY_EDITOR
        [Button]
#endif
        private void Validate()
        {
            InitValidator.ValidateStep(this);
        }
    }
}