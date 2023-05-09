using UnityEngine;

namespace PolyRocket.SO
{
    [CreateAssetMenu(menuName = "Rocket/Level Config")]
    public class LevelConfig: ScriptableObject
    {
        public float m_maxGameTime;
        public AnimationCurve m_cameraSpeedCurve;

        public float PlayerLaunchSpeed;
        public float PlayerClickPower;
        public float PlayerGravityScale;
        public float PlayerSpeedDcc;
        
        public float GetCameraSpeed(float time)
        {
            var normalizedTime = time / m_maxGameTime;
            return m_cameraSpeedCurve.Evaluate(normalizedTime);
        }
    }
}