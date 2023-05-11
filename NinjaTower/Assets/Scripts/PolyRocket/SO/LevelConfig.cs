﻿using UnityEngine;

namespace PolyRocket.SO
{
    [CreateAssetMenu(menuName = "Rocket/Level Config")]
    public class LevelConfig: ScriptableObject
    {
        public float m_maxGameTime;
        public AnimationCurve m_cameraSpeedCurve;

        // Player Relative Config
        public float LaunchSpeed;
        public float ClickPowerOrigin;
        public float ClickPowerPerBerry;
        
        public float GravityScale;
        public float SpeedDcc;

        public float m_MainForce;
        public float m_SideForce;
        
        public float GetCameraSpeed(float time)
        {
            var normalizedTime = time / m_maxGameTime;
            return m_cameraSpeedCurve.Evaluate(normalizedTime);
        }
    }
}