using Carotaa.Code;
using PolyRocket.UI;
using UnityEngine;

namespace PolyRocket.Game
{
    public class PrLauncher : MonoBehaviour
    {
        private void Awake()
        {
            PrCameraManager.Instance.WakeUp();
            UIManager.Instance.WakeUp();
            
            UIManager.Instance.Push<PrStartScreen>();
            
            Destroy(gameObject);
        }
    }
}