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
        public Collider2D col;

        public Transform aimOrigin;
        public Transform aimLength;

        private Vector2 _beginPos;
        private bool _isMoving;
        private PrGameLauncher _launcher;

        private bool _isSuccess;
        private bool _isControl;
        public void Init(PrGameLauncher launcher)
        {
            _launcher = launcher;
            _isMoving = false;
            SetAimVisible(false);
        }

        private void FixedUpdate()
        {
            var velocity = rb.velocity;
            rb.AddForce(-velocity * 0.2f, ForceMode2D.Force);
        }

        private void Update()
        {
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
            rb.velocity = Vector2.zero;
            // trigger event
            _isMoving = false;
            _isSuccess = false;
            CancelInvoke(nameof(StartCheckSpeed));
            _launcher.EPlayerMoveEnd.Raise();
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            _beginPos = eventData.position;
        }

        public void OnPointerUp(PointerEventData eventData)
        {
            if (!_isControl) return;
            
            // do something
        }

        public void OnBeginDrag(PointerEventData eventData)
        {
            if (_launcher.PlayerMoveLock) return;

            _isControl = true;
            
            SetAimVisible(true);
        }
        
        public void OnDrag(PointerEventData eventData)
        {
            if (!_isControl) return;
            
            var direct = GetAimDirect(eventData);
            var rotate = Quaternion.FromToRotation(Vector3.right, direct);

            var scale = direct.magnitude;
            aimOrigin.rotation = rotate;
            aimLength.localScale = Vector3.one;
        }

        public void OnEndDrag(PointerEventData eventData)
        {
            if (!_isControl) return;

            _isControl = false;
            var direct = GetAimDirect(eventData);

            _launcher.EPlayerMoveStart.Raise();
            Invoke(nameof(StartCheckSpeed), 1f); // start check if ball stops moving
            rb.AddForce(direct.normalized * 1000, ForceMode2D.Force);
            
            SetAimVisible(false);
        }

        private void StartCheckSpeed()
        {
            _isMoving = true;
        }

        private Vector2 GetAimDirect(PointerEventData eventData)
        {
            var pos = eventData.position;
            var direct = _beginPos - pos;
            return direct;
        }

        private void SetAimVisible(bool visible)
        {
            aimOrigin.gameObject.SetActive(visible);

        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (other.gameObject.CompareTag("Flag"))
            {
                // mark game over
                _isSuccess = true;
            }
        }
    }
}