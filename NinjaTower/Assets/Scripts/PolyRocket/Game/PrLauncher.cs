using BugsnagUnity;
using Carotaa.Code;
using PolyRocket.UI;
using UnityEngine;

namespace PolyRocket.Game
{
    public class PrLauncher : MonoBehaviour
    {
        private void Awake()
        {
#if DEBUG
            Bugsnag.Start("1234bd155fe29c256a154886bda08b32");
#endif
            PrCameraManager.Instance.WakeUp();
            UIManager.Instance.WakeUp();
            
            UIManager.Instance.Push<PrStartScreen>();
            
            Destroy(gameObject);
        }
    }
}