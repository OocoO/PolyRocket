using Carotaa.Code;
using UnityEngine;
using Random = System.Random;

namespace PolyRocket.Game
{
    public class PrActorGenerator : PrActor
    {
        public GameObject m_Prefab;
        public float m_DistanceInterval;
        private Vector3 _lastPos; // the camera pos when last generate

        private Random _random;
        private Camera _camera;

        public void Awake()
        {
            _camera = Level.m_LevelCamera;
            _lastPos = _camera.transform.position;

            _random = new Random(7);
        }

        public void Update()
        {
            var pos = _camera.transform.position;
            if ((pos - _lastPos).y > m_DistanceInterval)
            {
                _lastPos = pos;
                Generate();
            }
        }

        private void Generate()
        {
            var mainCam = _camera;

            var x = _random.Next(-100, 100) / 100f * mainCam.GetHalfWidth();
            var y = mainCam.transform.position.y + mainCam.orthographicSize + 10f;

            var go = Instantiate(m_Prefab, transform);
            go.transform.position = new Vector3(x, y);
            go.SetActive(true);
        }
    }
}