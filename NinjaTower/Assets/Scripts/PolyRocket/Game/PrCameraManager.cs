using Carotaa.Code;
using Cinemachine;
using UnityEngine;

namespace PolyRocket.Game
{
    public class PrCameraManager : MonoSingleton<PrCameraManager>
    {
        public CinemachineBrain m_camBrain;
        public CinemachineVirtualCamera m_virtualCam;
        public CinemachineConfiner2D m_camConfiner;
        
        public GameObject m_envRoot;
    }
}