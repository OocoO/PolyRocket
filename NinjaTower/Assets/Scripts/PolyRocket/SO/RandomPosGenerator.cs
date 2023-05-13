using Carotaa.Code;
using PolyRocket.Game;
using UnityEngine;

namespace PolyRocket.SO
{
    [CreateAssetMenu(menuName = "PolyRocket/Gen/RandomPos")]
    public class RandomPosGenerator: ElementGenerator
    {
        public GameObject m_Prefab;
        
        public float m_TimeInterval;
        public float m_DistanceInterval;
        
        private Vector3 _lastPos; // the camera pos when last generate

        private System.Random _random;
        private Camera _camera;
        private float _timer;
        
        public override void Init(PrLevel level)
        {
            base.Init(level);
            
            _random = new System.Random(7);
            _camera = Level.m_LevelCamera;
            _lastPos = _camera.transform.position;
        }
        
        
        public override void Update()
        {
            _timer -= Time.deltaTime;
            var pos = _camera.transform.position;
            var distanceCheck = (pos - _lastPos).y > m_DistanceInterval;
            var timeCheck = _timer <= 0f;
            if (!distanceCheck || !timeCheck) return;

            var success = TryGenerate();
            if (!success) return;
            
            _timer = m_TimeInterval;
            _lastPos = pos;
        }

        private bool TryGenerate()
        {
            var pos = GenPos(_camera);

            foreach (var actor in Level.Actors)
            {
                if ((actor.Position - pos).magnitude < 2f)
                    return false;
            }

            var go = Instantiate(m_Prefab, Level.transform);
            go.transform.position = pos;
            go.SetActive(true);

            return true;
        }

        private Vector2 GenPos(Camera mainCam)
        {
            var x = _random.Next(-100, 100) / 100f * mainCam.GetHalfWidth();
            var y = mainCam.transform.position.y + mainCam.orthographicSize + 10f;

            return new Vector2(x, y);
        }
    }
}