using System;
using System.Collections.Generic;
using UnityEngine;

namespace Carotaa.Code
{
    // Custom log class
    public class EventTrack : MonoSingleton<EventTrack>
    {
        public Dictionary<string, object> DebugParams;
        
        private void Awake()
        {
            DebugParams = new Dictionary<string, object>();
        }

        public static void LogError(object message)
        {
            Debug.LogError(message);
        }

        public static void Log(object message)
        {
            Debug.Log(message);
        }

        public static void LogParam(string key, object param)
        {
            Instance.DebugParams[key] = param;
        }
    }
}