using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Debug = UnityEngine.Debug;

namespace AppBootstrap.Runtime.Initialization
{
    public class InitProcess
    {
        private readonly Action _completeCallback;
        private readonly InitStepConfig[] _stepConfigs;
        private readonly Dictionary<string, object> _instances;
        private int _stepIndex;

        private Stopwatch _processTime;
        
        public InitProcess(InitStepsOrderConfig config, IEnumerable<object> injectables, Action completeCallback)
        {
            _completeCallback = completeCallback;
            _stepConfigs = config.StepConfigs.Where(x => x.IsEnabled).ToArray();
            _instances = injectables.ToDictionary(x => x.GetType().FullName);
        }

        public void Process()
        {
            _stepIndex = 0;
            NextStep();
        }

        private void NextStep()
        {
            _processTime = new Stopwatch();
            _processTime.Start();
            Debug.Log($"Start step [{CurrentStepConfig.StepKey}]");
            var stepProcess = new InitStepProcess(CurrentStepConfig.StepKey);
            stepProcess.Process(CurrentStepConfig.InfoList, _instances, StepCompleted);
        }

        private void StepCompleted()
        {
            Debug.Log($"Complete step [{CurrentStepConfig.StepKey}] [{_processTime.Elapsed.ToString()}]");
            ++_stepIndex;
            if (_stepIndex >= _stepConfigs.Length)
            {
                //Debug.Log($"All steps completed");
                _completeCallback?.Invoke();
                return;
            }

            NextStep();
        }

        private InitStepConfig CurrentStepConfig => _stepConfigs[_stepIndex];
    }
}