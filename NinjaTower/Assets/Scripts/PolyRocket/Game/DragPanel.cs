using System.Collections;
using System.Collections.Generic;
using Carotaa.Code;
using UnityEngine;
using UnityEngine.EventSystems;

namespace PolyRocket.Game
{
    public class DragPanel : MonoBehaviour, IDragHandler, IBeginDragHandler, IEndDragHandler, IPointerDownHandler
    {
        private PrGameLauncher _launcher;

        private Vector2 _startPos;
        
        public void Init(PrGameLauncher launcher)
        {
            _launcher = launcher;
        }


        public void OnDrag(PointerEventData eventData)
        {
            _launcher.SetCameraFollow(false);
        }

        public void OnBeginDrag(PointerEventData eventData)
        {
            var dMove = eventData.position - _startPos;
            var scale = ScreenUtility.World2ScreenMatrix(_launcher.mainCamera).lossyScale;
            var targetMove = dMove / scale;
            
            
        }

        public void OnEndDrag(PointerEventData eventData)
        {
            _launcher.SetCameraFollow(true);
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            _startPos = eventData.position;
        }
    }
}
