using AppBootstrap.Editor.Validator;
using AppBootstrap.Runtime;
using AppBootstrap.Runtime.Initialization;
using AppBootstrap.Runtime.Initialization.InitializationUtils;
using AppBootstrap.Runtime.Injector;
using UnityEditor;
using UnityEngine;

namespace AppBootstrap.Editor
{
    public static class BootstrapValidator
    {
        private static bool selectConfig = true;

        [MenuItem("Tools/Validate Bootstrap")]
        public static void ValidateBootstrapWithTimer()
        {
            ValidateBootstrap();
        }

        public static void ValidateBootstrap()
        {
            var appBootstrap = GameObject.FindObjectOfType<AppBootstrapper>(true);
            if (appBootstrap == null && Application.isPlaying)
            {
                Debug.LogError("Cant find AppBootstrapper on scene");
                return;
            }

            var configLoader = appBootstrap._configLoader;
            configLoader.LoadConfigs(AtConfigsLoaded);
        }

        private static void AtConfigsLoaded(InjectorConfig injConfig, InitStepsOrderConfig stepsConfig)
        {
            InjectValidator.ValidateConfig(injConfig);
            foreach (var step in stepsConfig.StepConfigs)
            {
                InitValidator.ValidateStep(step);
            }

            if (selectConfig)
                SelectConfig(injConfig);
        }

        private static void SelectConfig(InjectorConfig injConfig)
        {
            AssetDatabase.Refresh();
            EditorUtility.FocusProjectWindow();
            Selection.activeObject = injConfig;
        }

        [UnityEditor.Callbacks.DidReloadScripts]
        private static void OnScriptsReloaded()
        {
            selectConfig = false;
            ValidateBootstrap();
            selectConfig = true;
        }
    }
}