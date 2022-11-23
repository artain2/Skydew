using System;
using System.Collections.Generic;
using UnityEngine;

namespace AppBootstrap.Editor.Jarvis.Listeners
{
    [CreateAssetMenu(fileName = nameof(ListenersOrderLockConfig), menuName = "Configs/Jarvis/ListenersOrderLockConfig")]
    public class ListenersOrderLockConfig : ScriptableObject
    {
        public List<LockNode> Nodes;
    }

    [Serializable]
    public class LockNode
    {
        public string HostTypeName;
        public string FieldName;
        public string LockedItemTypeName;
        public int Lock;
    }
}