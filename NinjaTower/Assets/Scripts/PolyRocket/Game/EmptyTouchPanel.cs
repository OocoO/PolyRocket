using System.Collections;
using System.Collections.Generic;
using Carotaa.Code;
using UnityEngine;
using UnityEngine.EventSystems;

namespace PolyRocket.Game
{
    public class EmptyTouchPanel : Empty4Raycaster, 
        IDragHandler, 
        IBeginDragHandler, 
        IEndDragHandler, 
        IPointerDownHandler,
        IPointerClickHandler
    {
        private PrGameLauncher _launcher;
        
        public void Init(PrGameLauncher launcher)
        {
            _launcher = launcher;
            // temp: support multi
            Input.multiTouchEnabled = false;
        }

        public void OnBeginDrag(PointerEventData eventData)
        {
            _launcher.OnBeginDrag(eventData);
        }

        public void OnDrag(PointerEventData eventData)
        {
            _launcher.OnDrag(eventData);
        }

        public void OnEndDrag(PointerEventData eventData)
        {
            _launcher.OnEndDrag(eventData);
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            _launcher.OnPointerClick(eventData);
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            _launcher.OnPointerDown(eventData);
        }
    }
}
