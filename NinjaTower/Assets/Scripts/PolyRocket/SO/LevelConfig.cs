using UnityEngine;
using UnityEngine.Serialization;

namespace PolyRocket.SO
{
    [CreateAssetMenu(menuName = "Rocket/Level Config")]
    public class LevelConfig : ScriptableObject
    {
        [FormerlySerializedAs("m_maxGameTime")] public float m_CameraRefTime;
        public AnimationCurve m_cameraSpeedCurve;
        public float m_CameraSoftZoom;
        public float m_CameraHardZoom;

        // Player Relative Config
        public float LaunchSpeed;
        public float m_ForcePerBerry;

        public float GravityScale;
        public float SpeedDcc;

        public float m_MainForce;
        public float m_SideForce;
        public float m_LeftForceDirect;


        // Camera Relative Config
        public float m_MaxCameraSpeedScale; // relative to player speed
    }
}