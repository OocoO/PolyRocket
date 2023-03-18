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

        public static void LogError(object e)
        {
            Debug.LogError(e);
        }

        public static void LogTrace(object e)
        {
            Instance._trace.Add(e);
        }
    }
}