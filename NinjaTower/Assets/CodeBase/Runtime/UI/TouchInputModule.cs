using UnityEngine;
using UnityEngine.EventSystems;

namespace Carotaa.Code
{
    public class TouchInputModule : StandaloneInputModule
    {
        private uint _debugTouchFlag;

        // debug touch would last 32 frame
        private Vector2 _debugTouchPosition;
        private Vector2Int _screenSize;

        protected override void Awake()
        {
            base.Awake();

            _screenSize = ScreenUtility.Size;
        }


        public bool TryDebugTouch(Vector2 location)
        {
            if (_debugTouchFlag > 0)
                return false;

            _debugTouchFlag = 1;
            _debugTouchPosition = MathV.Clamp(location, Vector2.zero, _screenSize);

            return true;
        }


        public bool AbleDebugTouch()
        {
            return _debugTouchFlag == 0;
        }

        public override void Process()
        {
            if (!eventSystem.isFocused)
                return;

            // process debug touch first
            if (ProcessDebugTouchEvent()) return;

            if (ProcessTouchEventsAlt() || !input.mousePresent)
                return;

            ProcessMouseEvent();
        }

        private bool ProcessDebugTouchEvent()
        {
            var isTouchLast = _debugTouchFlag > 0;
            _debugTouchFlag <<= 1;
            var isTouch = _debugTouchFlag > 0;

            if (!isTouch && !isTouchLast) return false;

            var phase = isTouch ? isTouchLast ? TouchPhase.Moved : TouchPhase.Began : TouchPhase.Ended;

            var touch = new Touch
            {
                position = _debugTouchPosition,
                phase = phase
            };
            bool pressed;
            bool released;
            var pointerEventData = GetTouchPointerEventData(touch, out pressed, out released);


            ProcessTouchPress(pointerEventData, pressed, released);
            if (!released)
            {
                ProcessMove(pointerEventData);
                ProcessDrag(pointerEventData);
            }
            else
            {
                RemovePointerData(pointerEventData);
            }

            return true;
        }


        private bool ProcessTouchEventsAlt()
        {
            for (var index = 0; index < input.touchCount; ++index)
            {
                var touch = input.GetTouch(index);
                if (touch.type != TouchType.Indirect)
                {
                    bool pressed;
                    bool released;
                    var pointerEventData = GetTouchPointerEventData(touch, out pressed, out released);
                    ProcessTouchPress(pointerEventData, pressed, released);
                    if (!released)
                    {
                        ProcessMove(pointerEventData);
                        ProcessDrag(pointerEventData);
                    }
                    else
                    {
                        RemovePointerData(pointerEventData);
                    }
                }
            }

            return input.touchCount > 0;
        }
    }
}