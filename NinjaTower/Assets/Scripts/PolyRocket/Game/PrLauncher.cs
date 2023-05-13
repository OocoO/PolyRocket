using System.Diagnostics;
using BugsnagUnity;
using Carotaa.Code;
using PolyRocket.UI;
using UnityEngine;

namespace PolyRocket.Game
{
    public class PrLauncher : MonoBehaviour
    {
        public GameObject m_InGameDebugConsole;
        
        private void Awake()
        {
            ReleaseInit();
            PrCameraManager.Instance.WakeUp();
            UIManager.Instance.WakeUp();
            
            UIManager.Instance.Push<PrStartScreen>();
            
            Destroy(gameObject);
        }

        private void ReleaseInit()
        {
            if (IsDebugBuild()) return;
            
            
            Bugsnag.Start("1234bd155fe29c256a154886bda08b32");
            m_InGameDebugConsole.SetActive(false);
        }

        public static bool IsDebugBuild()
        {
#if DEBUG
            return true;
#else
            return false;
#endif
        }
    }
}