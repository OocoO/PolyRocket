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

        public void Init(PrGameLauncher launcher)
        {
            SetAimVisible(false);
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            _beginPos = eventData.position;
        }

        public void OnPointerUp(PointerEventData eventData)
        {
        }

        public void OnDrag(PointerEventData eventData)
        {
            var direct = GetAimDirect(eventData);
            var rotate = Quaternion.FromToRotation(Vector3.right, direct);

            var scale = direct.magnitude;
            aimOrigin.rotation = rotate;
            aimLength.localScale = Vector3.one;
        }

        public void OnBeginDrag(PointerEventData eventData)
        {
            
            SetAimVisible(true);
        }

        public void OnEndDrag(PointerEventData eventData)
        {
            var direct = GetAimDirect(eventData);
            
            rb.AddForce(direct.normalized * 100);
            
            SetAimVisible(false);
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
    }
}