using System;
using Carotaa.Code;
using UnityEngine;
using UnityEngine.Rendering.Universal;

namespace PolyRocket.Game
{
    public class PrCameraManager : MonoSingleton<PrCameraManager>
    {
        [SerializeField] private Camera m_MainCam;
        [SerializeField] private Camera m_UiCam;

        public void AddWorldCamera(Camera cam)
        {
            // cam.transform.SetParent(m_MainCam.transform);
            var cameraData = m_MainCam.GetUniversalAdditionalCameraData();
            cameraData.cameraStack.Clear();
            
            cameraData.cameraStack.Add(cam);
            cameraData.cameraStack.Add(m_UiCam);
        }

        public void RemoveWorldCamera(Camera cam)
        {
            var cameraData = m_MainCam.GetUniversalAdditionalCameraData();
            cameraData.cameraStack.Remove(cam);
        }
    }
}