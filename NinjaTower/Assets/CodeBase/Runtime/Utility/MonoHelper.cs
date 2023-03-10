using System;
using System.Collections.Generic;

namespace Carotaa.Code
{
	/// <summary>
	///     It's recommended use this class to replace Monobehaviour-event functions
	/// </summary>
	public class MonoHelper : MonoSingleton<MonoHelper>
    {
        private readonly Dictionary<Action<object>, object> _dirtyEvents = new Dictionary<Action<object>, object>();

        public readonly ShareEvent<bool> MonoApplicationFocus =
            ShareEvent.BuildEvent<bool>("MonoHelper.OnApplicationFocus");

        public readonly ShareEvent<bool> MonoApplicationPause =
            ShareEvent.BuildEvent<bool>("MonoHelper.OnApplicationPause");

        public readonly ShareEvent MonoLateUpdate = ShareEvent.BuildEvent("MonoHelper.OnLateUpdate");
        public readonly ShareEvent MonoUpdate = ShareEvent.BuildEvent("MonoHelper.OnUpdate");


        private void Update()
        {
            MonoUpdate.Raise();
        }

        private void LateUpdate()
        {
            MonoLateUpdate.Raise();
            InvokeDirtyEvents();
        }

        private void OnApplicationFocus(bool hasFocus)
        {
            MonoApplicationFocus.Raise(hasFocus);
        }

        private void OnApplicationPause(bool pauseStatus)
        {
            MonoApplicationPause.Raise(pauseStatus);
        }


        public void AddDirtyEvent(Action<object> sEvent, object param)
        {
            _dirtyEvents[sEvent] = param;
        }

        private void InvokeDirtyEvents()
        {
            foreach (var kvp in _dirtyEvents)
                try
                {
                    kvp.Key.Invoke(kvp.Value);
                }
                catch (Exception e)
                {
                    EventTrack.LogError(e);
                }

            _dirtyEvents.Clear();
        }
    }
}