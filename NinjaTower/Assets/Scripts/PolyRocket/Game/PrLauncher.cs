using Carotaa.Code;
using UnityEngine;

namespace PolyRocket.Game
{
    public class PrLauncher : MonoBehaviour
    {
        private void Start()
        {
            PrCameraManager.Instance.WakeUp();
            PrLevelManager.Instance.WakeUp();
            UIManager.Instance.WakeUp();
        }
    }
}