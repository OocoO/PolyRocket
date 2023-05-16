using UnityEngine;
using UnityEngine.Serialization;

namespace PolyRocket.SO
{
    [CreateAssetMenu(menuName = "PolyRocket/Level Config")]
    public class LevelConfig : ScriptableObject
    {
        [FormerlySerializedAs("m_maxGameTime")] public float m_CameraRefTime;

        public ElementGenerator[] m_Elements;
        
        public float m_CameraSoftZoom;
        public float m_CameraSoftDamp;

        // Player Relative Config
        public float LaunchSpeed;
        public float m_ForcePerBerry;

        public float GravityScale;
        public float SpeedDcc;

        public float m_MainForce;
        public float m_SideForce;
        public float m_LeftForceDirect;
        public float m_SuperForce;


        // Camera Relative Config
        public float m_MaxCameraSpeedScale; // relative to player speed
    }
}