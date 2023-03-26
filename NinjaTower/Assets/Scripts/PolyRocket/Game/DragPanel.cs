using System.Collections;
using System.Collections.Generic;
using Carotaa.Code;
using UnityEngine;
using UnityEngine.EventSystems;

namespace PolyRocket.Game
{
    public class DragPanel : Empty4Raycaster, IDragHandler, IBeginDragHandler, IEndDragHandler, IPointerDownHandler
    {
        private PrGameLauncher _launcher;

        private Vector2 _posLast;
        
        public void Init(PrGameLauncher launcher)
        {
            _launcher = launcher;
        }


        public void OnBeginDrag(PointerEventData eventData)
        {
            _launcher.SetCameraFollow(false);
            CancelInvoke(nameof(ResetCameraFollow));
        }

        public void OnDrag(PointerEventData eventData)
        {
            var dMove = _posLast - eventData.position;
            _posLast = eventData.position;
            
            _launcher.DragCamera(dMove);
        }

        public void OnEndDrag(PointerEventData eventData)
        {
            Invoke(nameof(ResetCameraFollow),1f);
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            _posLast = eventData.position;
        }

        private void ResetCameraFollow()
        {
            _launcher.SetCameraFollow(true);

        }
    }
}
