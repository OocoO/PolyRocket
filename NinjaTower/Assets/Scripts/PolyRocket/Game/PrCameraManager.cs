using System;
using Carotaa.Code;
using UnityEngine;

namespace PolyRocket.Game
{
    public class PrCameraManager : MonoSingleton<PrCameraManager>
    {
        public Camera m_MainCam;

        public Transform FollowTarget;

        private void Awake()
        {
            //
        }

        public void Update()
        {
            m_MainCam.transform.position = FollowTarget.position;
        }
    }
}