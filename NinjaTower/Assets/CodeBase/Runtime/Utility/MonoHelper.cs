using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Carotaa.Code
{
	/// <summary>
	///     It's recommended use this class to replace Monobehaviour-event functions
	/// </summary>
	public class MonoHelper : MonoSingleton<MonoHelper>
    {
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
        }

        private void OnApplicationFocus(bool hasFocus)
        {
            MonoApplicationFocus.Raise(hasFocus);
        }

        private void OnApplicationPause(bool pauseStatus)
        {
            MonoApplicationPause.Raise(pauseStatus);
        }
        
        public void DispatchAfterSeconds(Action action, float seconds)
        {
            StartCoroutine(DispatchAfterSecondsCoroutine(action, seconds));
        }
        
        private IEnumerator DispatchAfterSecondsCoroutine(Action action, float seconds)
        {
            yield return new WaitForSeconds(seconds);
            action?.Invoke();
        }
    }
}