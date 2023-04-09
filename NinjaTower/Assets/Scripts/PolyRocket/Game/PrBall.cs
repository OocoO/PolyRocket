using System;
using Carotaa.Code;
using PolyRocket.Game;
using UnityEngine;
using UnityEngine.EventSystems;

namespace PolyRocket
{
    public class PrBall : MonoBehaviour, 
        IPointerDownHandler,
        IPointerUpHandler, 
        IBeginDragHandler,
        IEndDragHandler,
        IDragHandler
    {
        public Rigidbody2D rb;
        public CircleCollider2D col;
        public PrBallPhysics ballPhysics;
        public Transform ballTouchZoom;

        public Transform aimOrigin;
        public Transform aimLength;

        private bool _isMoving;
        private PrGameLauncher _launcher;

        private bool _isSuccess;
        private bool _isControl;
        public void Init(PrGameLauncher launcher)
        {
            _launcher = launcher;
            _isMoving = false;
            ballPhysics.Init(this);

            rb.interpolation = RigidbodyInterpolation2D.Interpolate;
            SetAimVisible(false);
        }

        private void FixedUpdate()
        {
            var velocity = rb.velocity;
            rb.AddForce(-velocity * _launcher.speedDecrease, ForceMode2D.Force);
        }

        private void Update()
        {
            UpdateTouchZoom();
            CheckIsSuccess();
            CheckIsFailed();
        }

        private void CheckIsFailed()
        {
            if (!_isMoving) return;
            
            var velocity = rb.velocity;
            if (velocity.magnitude <= 0.5f)
            {
                StopMove();
            }
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
            _isMoving = false;
            _isSuccess = false;
            CancelInvoke(nameof(StartCheckSpeed));
        }

        public void OnPointerDown(PointerEventData eventData)
        {
        }

        public void OnPointerUp(PointerEventData eventData)
        {
            if (!_isControl) return;
            
            // do something
        }

        public void OnBeginDrag(PointerEventData eventData)
        {
            if (_launcher.PlayerMoveLock) return;

            SetControl(true);
        }
        
        public void OnDrag(PointerEventData eventData)
        {
            if (!_isControl) return;
            
            var direct = GetAimDirect(eventData);
            var rotate = Quaternion.FromToRotation(Vector3.right, direct);

            var success = TryGetForceScale(eventData, out var scale);
            SetAimVisible(success);

            scale *= 0.5f;

            var dis = scale * 4f + 1f;
            aimOrigin.rotation = rotate;
            aimLength.localScale = new Vector3(1f + scale, 1f - scale, 1f);
            aimLength.localPosition = Vector3.right * dis;
        }

        public void OnEndDrag(PointerEventData eventData)
        {
            if (!_isControl) return;

            SetControl(false);
            
            var shot = TryGetForceScale(eventData, out var scale);

            if (!shot) return;
            
            var direct = GetAimDirect(eventData);

            _launcher.EPlayerMoveStart.Raise();
            Invoke(nameof(StartCheckSpeed), 1f); // start check if ball stops moving
            ResetSpeed();
            rb.AddForce(direct.normalized * _launcher.maxShotForce * scale, ForceMode2D.Force);
            
            SetAimVisible(false);
        }

        private void StartCheckSpeed()
        {
            _isMoving = true;
        }

        private Vector2 GetAimDirect(PointerEventData eventData)
        {
            var pos = eventData.position;
            var startPos = (Vector2) _launcher.mainCamera.WorldToScreenPoint(rb.position);
            var direct = startPos - pos;
            return direct;
        }

        private bool TryGetForceScale(PointerEventData eventData, out float scale)
        {
            var direct = GetAimDirect(eventData);
            scale = 0f;
            var mag = direct.magnitude - _launcher.safeZoom;
            if (mag < 0f)
            {
                return false;
            }
            scale = mag * 5f / ScreenUtility.Width;
            scale = Mathf.Clamp(scale, 0.001f, 1f);
            return true;
        }

        private void SetAimVisible(bool visible)
        {
            aimOrigin.gameObject.SetActive(visible);

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

        private void SetControl(bool isControl)
        {
            _isControl = isControl;
            _launcher.SetPhysicsPause(isControl);
        }

        private void UpdateTouchZoom()
        {
            ballTouchZoom.position = ballPhysics.transform.position;
        }

        private static bool IsTrapTag(GameObject go)
        {
            return go.CompareTag("StaticTrap") || go.CompareTag("DynamicTrap");
        }
    }
}