using System;
using Carotaa.Code;
using UnityEngine;

namespace PolyRocket.Game
{
    public class PrGlobal
    {
        public float PlayerDashDistance = 8f;
        
        public Action<GameObject> EPlayerMoveTriggerFlag;

        public Action EPlayerTriggerTrap;

        public float ballScreenRadius;

        public Camera mainCamera;
        
        public Vector2 Screen2WorldPosition(Vector2 screenPos)
        {
            var mat = ScreenUtility.World2ScreenMatrix(mainCamera).inverse;
            var pos = mat.MultiplyPoint3x4(screenPos);
            return pos;
        }
    }
}