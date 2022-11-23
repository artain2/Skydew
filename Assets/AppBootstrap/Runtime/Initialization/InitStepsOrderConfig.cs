using System.Collections.Generic;
using UnityEngine;

namespace AppBootstrap.Runtime.Initialization
{
    [CreateAssetMenu(fileName = nameof(InitStepsOrderConfig), menuName = "Configs/Bootstrap/" + nameof(InitStepsOrderConfig))]
    public class InitStepsOrderConfig : ScriptableObject
    {
        public List<InitStepConfig> StepConfigs = new List<InitStepConfig>();
    }
}