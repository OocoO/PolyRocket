using System;
using Carotaa.Code;
using UnityEngine;
using UnityEngine.EventSystems;

namespace PolyRocket.Game
{
    [RequireComponent(typeof(CanvasRenderer))]
    public class EmptyTouchPanel : Empty4Raycaster, 
        IDragHandler, 
        IBeginDragHandler, 
        IEndDragHandler, 
        IPointerDownHandler,
        IPointerClickHandler,
        IPointerUpHandler
    {
        public event Action<PointerEventData> EBeginDrag;
        public event Action<PointerEventData> EDrag;
        public event Action<PointerEventData> EEndDrag;
        public event Action<PointerEventData> EPointerClick;
        public event Action<PointerEventData> EPointerDown;
        public event Action<PointerEventData> EPointerUp;

        public void OnBeginDrag(PointerEventData eventData)
        {
            EBeginDrag?.Invoke(eventData);
        }

        public void OnDrag(PointerEventData eventData)
        {
            EDrag?.Invoke(eventData);
        }

        public void OnEndDrag(PointerEventData eventData)
        {
            EEndDrag?.Invoke(eventData);
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            EPointerClick?.Invoke(eventData);
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            EPointerDown?.Invoke(eventData);
        }

        public void OnPointerUp(PointerEventData eventData)
        {
            EPointerUp?.Invoke(eventData);
        }
    }
}
