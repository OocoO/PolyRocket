using System;
using Carotaa.Code;
using PolyRocket.Game;
using UnityEngine;
using UnityEngine.EventSystems;

namespace PolyRocket
{
    public class PrBall : MonoBehaviour
    {
        public Rigidbody2D rb;
        public CircleCollider2D col;
        public PrBallPhysics ballPhysics;
        
        private PrGameLauncher _launcher;

        private bool _isSuccess;
        private bool _isControl;
        public void Init(PrGameLauncher launcher)
        {
            _launcher = launcher;
            ballPhysics.Init(this);

            rb.interpolation = RigidbodyInterpolation2D.Interpolate;
        }

        private void FixedUpdate()
        {
            var velocity = rb.velocity;
            rb.AddForce(-velocity * _launcher.speedDecrease, ForceMode2D.Force);
        }

        private void Update()
        {
            CheckIsSuccess();
        }

        private void CheckIsSuccess()
        {
            if (_isSuccess)
            {
                _launcher.EPlayerMoveToTarget.Raise();
                // stop move
                StopMove();
            }
        }

        private void StopMove()
        {
            ResetSpeed();
            // trigger event
            _isSuccess = false;
        }

        public void OnWindBlow(PointerEventData eventData)
        {
            var scale = GetForceScale(eventData);
            
            var direct = GetAimDirect(eventData);

            _launcher.EPlayerMoveStart.Raise();
            rb.AddForce(direct.normalized * _launcher.maxShotForce * scale, ForceMode2D.Force);
        }

        private Vector2 GetAimDirect(PointerEventData eventData)
        {
            var pos = eventData.position;
            var startPos = (Vector2) _launcher.mainCamera.WorldToScreenPoint(rb.position);
            var direct = startPos - pos;
            return direct;
        }

        private float GetForceScale(PointerEventData eventData)
        {
            var direct = GetAimDirect(eventData);
            var scale = 1f;
            var mag = direct.magnitude - _launcher.ballScreenRadius;
            if (mag < 0f)
            {
                return scale;
            }

            return _launcher.ballScreenRadius / direct.magnitude;
        }

        private void ResetSpeed()
        {
            rb.velocity = Vector2.zero;
        }

        public void OnPhysicsTrigger(Collider2D other)
        {
            var otherGo = other.gameObject;
            if (otherGo.CompareTag("Flag"))
            {
                // mark game over
                _isSuccess = true;
            }
            else if (IsTrapTag(otherGo))
            {
                // stop move
                StopMove();
                _launcher.OnPlayerTriggerTrap();
            }
        }

        private static bool IsTrapTag(GameObject go)
        {
            return go.CompareTag("StaticTrap") || go.CompareTag("DynamicTrap");
        }
    }
}