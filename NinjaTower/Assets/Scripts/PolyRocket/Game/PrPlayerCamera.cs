using DG.Tweening;
using UnityEngine;

namespace PolyRocket.Game
{
    public class PrPlayerCamera
    {
        private const float OrthographicCameraSize = 30f;
        
        private Transform _camTrans;
        private Camera _mainCam;

        private PrPlayer _player;
        private Rigidbody2D _rb;
        private float _cameraSize;
        private float _time;

        public PrPlayerCamera(PrPlayer player, Camera camera)
        {
            _player = player;
            _mainCam = camera;
            // ReSharper disable once PossibleNullReferenceException
            _rb = _mainCam.GetComponent<Rigidbody2D>();
            _camTrans = _mainCam.transform;
            _time = 0f;

            SetCameraSize(6f);
            InitPosition();
        }

        private void InitPosition()
        {
            var offset = _cameraSize * 0.4f;
            var pos = _camTrans.position;
            pos.y = _player.Position.y + offset;
            _camTrans.position = pos;
        }
        
        public void FixedUpdate()
        {
            _time += Time.deltaTime * Time.timeScale;
            
            var vy = _player.Level.Config.GetCameraSpeed(_time);
            _rb.velocity = Vector2.up * vy;
        }

        public void StartZoomOutAnim()
        {
            var tweener = DOTween.To(() => _cameraSize, SetCameraSize, 12f, 5f);
            tweener.SetEase(Ease.OutCubic);
        }

        private void RefreshCameraSize()
        {
            var size = PlayerSpeed2CameraSize();
            SetCameraSize(size);
        }
        private void SetCameraSize(float halfHeight)
        {
            _cameraSize = halfHeight;
            _mainCam.orthographicSize = halfHeight;
            var scale = halfHeight / OrthographicCameraSize;
            _camTrans.localScale = Vector3.one * scale;
        }

        private float PlayerSpeed2CameraSize()
        {
            var speedY = _player.Velocity.y;
            return 5f + speedY;
        }
    }
}