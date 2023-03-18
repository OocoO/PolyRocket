using System;
using UnityEngine;

namespace Carotaa.Code
{
    // Custom log class
    public class EventTrack : Singleton<EventTrack>
    {
        private const int TraceSize = 100;
        private CycleArray<object> _trace;

        protected override void OnCreate()
        {
            _trace = new CycleArray<object>(TraceSize);
        }

        public static void OnRuntimeInit()
        {
            Instance.WakeUp();
        }

        public static void LogError(object message, Exception e = null)
        {
            Debug.LogError(e);
        }

        public static void Log(object message)
        {
            Debug.Log(message);
        }
    }
}