using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEditor;
using UnityEngine;

namespace Carotaa.Code.Editor
{
    [CustomEditor(typeof(MonoBehaviourAlt), true)]
    public class MonoBehaviourAltEditor : UnityEditor.Editor
    {
        private const string MethodName = "DebugTrigger";
        private List<Tuple<string, MethodInfo>> _debugMethods;

        private void OnEnable()
        {
            if (_debugMethods == null)
            {
                _debugMethods = new List<Tuple<string, MethodInfo>>();
                var mInfos = target.GetType().GetMethods();
                foreach (var info in mInfos)
                    if (info.Name.StartsWith(MethodName))
                    {
                        var btnName = GetButtonName(info);
                        _debugMethods.Add(new Tuple<string, MethodInfo>(btnName, info));
                    }
            }
        }

        private static string GetButtonName(MethodInfo info)
        {
            var attribute = (DebugButtonNameAttribute) info.GetCustomAttribute(typeof(DebugButtonNameAttribute));
            if (attribute != null) return attribute.AltName;

            return info.Name;
        }

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            if (!Application.isPlaying) return;

            foreach (var info in _debugMethods)
                if (GUILayout.Button(info.Item1))
                    info.Item2.Invoke(target, null);
        }
    }
}