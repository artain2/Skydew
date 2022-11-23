using System;
using System.Collections.Generic;
using UnityEngine;

namespace AppBootstrap.Editor.Jarvis.Utils
{
    [CreateAssetMenu(fileName = nameof(InjectionFilesCache), menuName = "Configs/Jarvis/InjectionFilesCache")]
    public class InjectionFilesCache : ScriptableObject
    {
        public List<Node> Nodes = new List<Node>();

        [Serializable]
        public class Node
        {
            public string FullName;
            public TextAsset CodeFile;
        }
    }
}