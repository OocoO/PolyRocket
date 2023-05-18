using System;
using Carotaa.Code;
using DG.Tweening;
using PolyRocket.Game.Actor;
using UnityEngine;

namespace PolyRocket.Game
{
    public class PrPlayerCamera
    {
        private const float CameraSizeMin = 6f;
        private const float CameraSizeMid = 12f;
        private const float CameraSizeMax = 24f;
        
        private Transform _camTrans;
        private Camera _mainCam;

        private PrPlayer _player;
        private Rigidbody2D _rb;
        private float _cameraHalfHeight;
        private float _cameraBaseSpeed;
        private PrLevel _level;
        private float _currentVelocity;
        private float _currentSizeSpeed;
       

        public PrPlayerCamera(PrPlayer player, Camera camera)
        {
            _player = player;
            _mainCam = camera;
            _level = player.Level;
            // ReSharper disable once PossibleNullReferenceException
            _rb = _mainCam.GetComponent<Rigidbody2D>();
            _camTrans = _mainCam.transform;
            _cameraBaseSpeed = 0f;

            SetCameraSize(CameraSizeMin);
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
            // Damp camera zoom
            var targetSize = State2CameraSize(_player.StateMachine.State);
            var nextSize = Mathf.SmoothDamp(_cameraHalfHeight, targetSize, ref _currentSizeSpeed, 2f);
            SetCameraSize(nextSize);
        }

        public void FixedUpdate()
        {
            var deltaTime = Time.fixedDeltaTime;
            StepCameraSpeed(deltaTime);
            
            var config = _player.Level.Config;
            var baseSpeed = _cameraBaseSpeed;

            var camPos = _rb.position.y;
            var camBottom = camPos - _cameraHalfHeight;
            var camHeight = _cameraHalfHeight * 2f;
            var playerPos = _player.Position.y;
            var targetPos = playerPos + (0.5f - config.m_CameraSoftZoom) * camHeight;
            var pos = (playerPos - camBottom) / camHeight;
            
            var inSoftZoom = pos > config.m_CameraSoftZoom;
            
            var nextPos = camPos + baseSpeed * deltaTime;

            if (inSoftZoom)
            {
                nextPos = Mathf.SmoothDamp(camPos, targetPos, ref _currentVelocity, 
                    config.m_CameraSoftDamp, float.MaxValue,
                    deltaTime);
            }

            _rb.MovePosition(Vector2.up * nextPos);
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
            var vN = _cameraBaseSpeed;
            var h = _level.MaxCameraSpeed;
            
            var tN = (h / (h - vN) - 1f) * 0.2f * refTime;
            var tN1 = tN + deltaTime;
            var vN1 = h - h / (5f * tN1 / refTime + 1f);
            _cameraBaseSpeed = vN1;
            
            EventTrack.LogParam($"CameraBaseSpeed", _cameraBaseSpeed);
        }

        private static float State2CameraSize(PrPlayer.State state)
        {
            switch (state)
            {
                case PrPlayer.State.Idle:
                    return CameraSizeMin;
                case PrPlayer.State.Launch:
                    return CameraSizeMid;
                case PrPlayer.State.SuperRocket:
                    return CameraSizeMax;
                default:
                    throw new ArgumentOutOfRangeException(nameof(state), state, null);
            }
        }
    }
}