using System;
using UnityEngine;

namespace Carotaa.Code
{
    public static class GameLauncher
    {
        public static event Action Launch;

        public static event Action LauncherOver;

        // Write all your init method here
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
        private static void Main()
        {
            // Utility
            EventTrack.OnRuntimeInit();
            GlobalVariable.OnRuntimeInit();
            SqlUtility.OnRuntimeInit();
            MonoHelper.Instance.WakeUp();

            // Finally start Loading
            // choose a launcher
            Launch?.Invoke();
            LauncherOver?.Invoke();
        }
    }
}