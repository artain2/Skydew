using System;
using System.IO;
using UnityEditor;
using UnityEngine;

namespace Utils
{
    public class ScriptableObjectCreator
    {
        private const string MenuName = "Tools/CreateSO %&s";

        [MenuItem(MenuName)]
        public static void CreateScriptableObjectByScript()
        {
            if (!ValidateCreateScriptableObjectByScript())
                return;

            var ta = Selection.objects[0] as TextAsset;
            var inst = ScriptableObject.CreateInstance(ta.name);
            if (inst == null)
                return;

            var taPath = AssetDatabase.GetAssetPath(ta);
            var directory = Path.GetDirectoryName(taPath);
            var soPath = $"{directory}/{ta.name}.asset";
            AssetDatabase.CreateAsset(inst, soPath);
        }

        [MenuItem(MenuName, true)]
        private static bool ValidateCreateScriptableObjectByScript()
        {
            var selected = Selection.objects;
            if (selected.Length != 1)
                return false;

            if (!(selected[0] is TextAsset ta))
                return false;

            var typeName = $"{GetNamespace(ta.text)}.{ta.name}";
            var isSo = typeof(ScriptableObjectCreator).Assembly.GetType(typeName).IsSubclassOf(typeof(ScriptableObject));
            return isSo;
        }

        private static string GetNamespace(string text)
        {
            var nsString = "namespace ";
            var nsEntrance = text.IndexOf(nsString);
            if (nsEntrance < 0)
            {
                return "";
            }

            var pointer = nsEntrance + nsString.Length;
            var braceEntrance = text.IndexOf("{", pointer);
            var substr = text.Substring(pointer, braceEntrance-pointer);
            var result = substr.Trim();
            return result;
        }
    }
}