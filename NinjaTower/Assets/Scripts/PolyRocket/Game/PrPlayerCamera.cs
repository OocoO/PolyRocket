using Carotaa.Code;
using DG.Tweening;
using PolyRocket.Game.Actor;
using UnityEngine;

namespace PolyRocket.Game
{
    public class PrPlayerCamera
    {
        private Transform _camTrans;
        private Camera _mainCam;

        private PrPlayer _player;
        private Rigidbody2D _rb;
        private float _cameraHalfHeight;
        private float _cameraSpeed;
        private PrLevel _level;

        public PrPlayerCamera(PrPlayer player, Camera camera)
        {
            _player = player;
            _mainCam = camera;
            _level = player.Level;
            // ReSharper disable once PossibleNullReferenceException
            _rb = _mainCam.GetComponent<Rigidbody2D>();
            _camTrans = _mainCam.transform;
            _cameraSpeed = 0f;

            SetCameraSize(6f);
            InitPosition();
        }

        private void InitPosition()
        {
            var offset = _cameraHalfHeight * 0.4f;
            var pos = _camTrans.position;
            pos.y = _player.Position.y + offset;
            _camTrans.position = pos;
        }
        
        public void Update()
        {
            // do something
        }

        public void FixedUpdate()
        {
            StepCameraSpeed(Time.fixedDeltaTime * Time.timeScale);
            
            var config = _player.Level.Config;
            var baseSpeed = _cameraSpeed;

            var camBottom = _rb.position.y - _cameraHalfHeight;
            var camHeight = _cameraHalfHeight * 2f;
            var playerPos = _player.Position.y;
            var pos = (playerPos - camBottom) / camHeight;
            
            var inSoftZoom = pos > config.m_CameraSoftZoom;
            var softScaler = (pos - config.m_CameraSoftZoom) / (config.m_CameraHardZoom - config.m_CameraSoftZoom);

            if (inSoftZoom)
            {
                _rb.velocity = Vector2.up * Mathf.Max(_player.Velocity.y * softScaler, baseSpeed);
            }
            else
            {
                _rb.velocity = Vector2.up * (baseSpeed);
            }
        }

        public void StartZoomOutAnim()
        {
            var tweener = DOTween.To(() => _cameraHalfHeight, SetCameraSize, 12f, 5f);
            tweener.SetEase(Ease.OutCubic);
        }

        private void RefreshCameraSize()
        {
            var size = PlayerSpeed2CameraSize();
            SetCameraSize(size);
        }
        private void SetCameraSize(float halfHeight)
        {
            _cameraHalfHeight = halfHeight;
            _level.SetCameraSize(halfHeight);
        }

        private float PlayerSpeed2CameraSize()
        {
            var speedY = _player.Velocity.y;
            return 5f + speedY;
        }

        // V(n+1) = F(V(n), MaxCameraSpeed)
        // V = h - h / (5 * t / ref + 1) where h : MaxCameraSpeed
        // t(n+1) = t(n) + deltaTime
        private void StepCameraSpeed(float deltaTime)
        {
            var refTime = _level.RefTime;
            var vN = _cameraSpeed;
            var h = _level.MaxCameraSpeed;
            
            var tN = (h / (h - vN) - 1f) * 0.2f * refTime;
            var tN1 = tN + deltaTime;
            var vN1 = h - h / (5f * tN1 / refTime + 1f);
            _cameraSpeed = vN1;
            
            EventTrack.LogParam($"CameraBaseSpeed", _cameraSpeed);
        }
    }
}