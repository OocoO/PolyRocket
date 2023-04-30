using System;
using Carotaa.Code;
using UnityEngine;

namespace PolyRocket.Game
{
    public class PrGlobal
    {
        // player related
        public float PlayerDashDistance = 8f;
        
        public Action<PrActor> EPlayerMoveTriggerFlag;

        public ShareVariable<int> VPlayerDashLevel = ShareEvent.BuildVariable<int>(nameof(VPlayerDashLevel));

        public Action EPlayerTriggerTrap;

        public float BallScreenRadius;

        public Camera MainCamera;
        
        public Vector2 Screen2WorldPosition(Vector2 screenPos)
        {
            var mat = ScreenUtility.World2ScreenMatrix(MainCamera).inverse;
            var pos = mat.MultiplyPoint3x4(screenPos);
            return pos;
        }
    }
}