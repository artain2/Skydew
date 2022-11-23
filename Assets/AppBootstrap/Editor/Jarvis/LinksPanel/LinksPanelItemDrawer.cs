using System;
using System.Collections.Generic;
using AppBootstrap.Runtime.Injector.InjectUtils;
using EGL = UnityEditor.EditorGUILayout;
using EG = UnityEditor.EditorGUI;
using GL = UnityEngine.GUILayout;
using Object = UnityEngine.Object;

namespace AppBootstrap.Editor.Jarvis.Links
{
    public class LinksPanelItemDrawer
    {
        private List<LinksInjectingInfo> _links;
        private bool _expanded = false;
        private string _serviceName;
        private Action _atChange;
        
        public string ServiceName => _serviceName;

        public void SetLinks(List<LinksInjectingInfo> links) => _links = links;
        public void SetName(string serviceName) => _serviceName = serviceName;
        public void SetChangeAction(Action atChange) => _atChange = atChange;

        public void Draw()
        {
            EGL.BeginVertical("helpBox");
            _expanded = EGL.Foldout(_expanded, _serviceName);
            if (!_expanded)
            {
                EGL.EndVertical();
                return;
            }

            var hasChange = false;
            for (var i = 0; i < _links.Count; i++)
            {
                EGL.BeginHorizontal();
              //  EGL.Space(2f);
                EG.BeginChangeCheck();
                var newObj = EGL.ObjectField(_links[i].FieldName, _links[i].Value, typeof(Object), false);
                if (EG.EndChangeCheck())
                {
                    _links[i].Value = newObj;
                    hasChange = true;
                }

                EGL.EndHorizontal();
            }

            if (hasChange)
            {
                _atChange?.Invoke();
            }

            EGL.EndVertical();
        }
    }
}