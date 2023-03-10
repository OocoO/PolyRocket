using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Carotaa.Code
{
    public class UIManager : Singleton<UIManager>
    {
        private const int PanelInterval = 100;

        // Used to support multi screen resolution
        private static readonly Dictionary<ScreenProfile, Vector2Int> ScreenLut =
            new Dictionary<ScreenProfile, Vector2Int>
            {
                {ScreenProfile.Mobile, new Vector2Int(1080, 1920)},
                {ScreenProfile.Pad, new Vector2Int(2048, 1536)}
                // { ScreenProfile.Notch, new Vector2Int(2532, 1170)},
            };

        private Camera _camera;
        private LinkedListNode<PageRecord> _currentClickableNode;

        private LinkedList<PendingRecordBase> _operationsQueue;
        private ScreenProfile _profile;
        private GameObject _uiRoot;
        private LinkedList<PageRecord> _uiStack;

        public bool IsReady { get; private set; }

        public void Preload<T>() where T : ControllerBase
        {
            EnqueueOperation(typeof(T), PendingRecordBase.Code.Preload);
        }

        public void Push<T>() where T : ControllerBase
        {
            EnqueueOperation(typeof(T), PendingRecordBase.Code.Active);
        }

        public void Hide<T>() where T : ControllerBase
        {
            EnqueueOperation(typeof(T), PendingRecordBase.Code.DeActive);
        }

        public void Pop<T>() where T : ControllerBase
        {
            EnqueueOperation(typeof(T), PendingRecordBase.Code.Destroy);
        }

        private void EnqueueOperation(Type type, PendingRecordBase.Code code)
        {
            var record = BuildRecord(type, code);
            EnqueueOperation(record);
        }

        private void EnqueueOperation(PendingRecordBase operation)
        {
            // Directly add, legal check when processing
            _operationsQueue.AddLast(operation);
        }

        public override void WakeUp()
        {
            IsReady = true;
        }

        protected override void OnCreate()
        {
            _currentClickableNode = null;
            _uiStack = new LinkedList<PageRecord>();
            _operationsQueue = new LinkedList<PendingRecordBase>();
            _uiRoot = new GameObject("UIRoot");

            // Maybe: use a different camera?
            _camera = CameraUtility.Main;

            var screenSize = ScreenUtility.Size;
            _profile = GetMatchProfile(screenSize);

            var device = _profile == ScreenProfile.Mobile ? MultiScreenRect.Device.Mobile : MultiScreenRect.Device.Pad;
            MultiScreenRect.SetRectProfileValue(device);

            var canvas = _uiRoot.AddComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceCamera;
            canvas.worldCamera = _camera;

            var scaler = _uiRoot.AddComponent<CanvasScaler>();
            scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            scaler.screenMatchMode = CanvasScaler.ScreenMatchMode.MatchWidthOrHeight;
            scaler.scaleFactor = 0f;
            scaler.referenceResolution = ScreenLut[_profile];

            MonoHelper.Instance.MonoLateUpdate.Subscribe(MonoLateUpdate);
        }

        private void MonoLateUpdate()
        {
            var status = new State(_operationsQueue, _uiStack);

            var prt = _operationsQueue.First;
            while (prt != null)
            {
                var record = prt.Value;
                var code = record.Tick(status);
                if (code == PendingRecordBase.PendingStatus.Out)
                {
                    _operationsQueue.Remove(prt);
                    ProcessRecord(record);
                    break;
                }

                if (code == PendingRecordBase.PendingStatus.Discard)
                {
                    _operationsQueue.Remove(prt);
                }

                prt = prt.Next;
            }
        }

        private void ProcessRecord(PendingRecordBase record)
        {
            switch (record.PageOperation)
            {
                case PendingRecordBase.Code.Preload:
                    ProcessPreload(record);
                    break;
                case PendingRecordBase.Code.Active:
                    ProcessActive(record);
                    break;
                case PendingRecordBase.Code.DeActive:
                    ProcessDeActive(record);
                    break;
                case PendingRecordBase.Code.Destroy:
                    ProcessDestroy(record);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        private void ProcessPreload(PendingRecordBase record)
        {
            var pageRecord = _uiStack.Find(x => x.PageType == record.PageType);
            if (pageRecord != null) return;

            var nRecord = new PageRecord(record.PageType);

            var i = 1;
            if (_uiStack.Count <= 0)
            {
                _uiStack.AddFirst(nRecord);
            }
            else
            {
                var target = _uiStack.First;
                var next = target;
                while (next != null)
                {
                    if (next.Value.GetLayer() > record.Layer) break;

                    i++;
                    target = next;
                    next = target.Next;
                }

                _uiStack.AddAfter(target, nRecord);
            }

            nRecord.Init(_uiRoot.transform, i * PanelInterval);
            nRecord.OnCreate(record);
        }

        private void ProcessActive(PendingRecordBase record)
        {
            ProcessPreload(record);

            // ReSharper disable once PossibleNullReferenceException
            var pageRecord = _uiStack.Find(x => x.PageType == record.PageType);

            if (pageRecord.Value.IsVisible) return;

            pageRecord.Value.Show(record);

            if (FindLastActive() == pageRecord)
            {
                _currentClickableNode?.Value.SetClickable(false);
                pageRecord.Value.SetClickable(true);
                _currentClickableNode = pageRecord;
            }
        }

        private void ProcessDeActive(PendingRecordBase record)
        {
            var pageRecord = _uiStack.Find(x => x.PageType == record.PageType);

            if (!pageRecord.Value.IsVisible) return;

            if (_currentClickableNode == pageRecord)
            {
                _currentClickableNode.Value.SetClickable(false);
                var prvNode = pageRecord.Previous;
                prvNode?.Value.SetClickable(true);
                _currentClickableNode = prvNode;
            }

            pageRecord.Value.Hide(record);
        }

        private void ProcessDestroy(PendingRecordBase record)
        {
            var pageRecord = _uiStack.Find(x => x.PageType == record.PageType);

            ProcessDeActive(record);

            pageRecord.Value.OnDestroy(record);
            _uiStack.Remove(pageRecord);
        }

        private LinkedListNode<PageRecord> FindLastActive()
        {
            var last = _uiStack.Last;
            while (last != null)
            {
                if (last.Value.IsVisible) return last;

                last = last.Previous;
            }

            return null;
        }

        public State GetState()
        {
            return new State(_operationsQueue, _uiStack);
        }

        private static PendingRecordBase BuildRecord(Type type, PendingRecordBase.Code operation)
        {
            if (type.IsSubclassOf(typeof(SlideBase))) return new PendingSlide(type, operation);

            if (type.IsSubclassOf(typeof(PopBase))) return new PendingPop(type, operation);

            if (type.IsSubclassOf(typeof(ToastBase))) return new PendingToast(type, operation);

            throw new Exception($"Unsupported UI type {type}");
        }

        private static ScreenProfile GetMatchProfile(Vector2Int resolution)
        {
            var sizeMobile = ScreenLut[ScreenProfile.Mobile];
            var sizePad = ScreenLut[ScreenProfile.Pad];

            var aspectMobile = (float) sizeMobile.y / sizeMobile.x;
            var aspectPad = (float) sizePad.y / sizePad.x;
            var aspectCurrent = (float) resolution.y / resolution.x;

            var diffMobile = Mathf.Abs(aspectCurrent - aspectMobile);
            var diffPad = Mathf.Abs(aspectCurrent - aspectPad);

            return diffMobile < diffPad ? ScreenProfile.Mobile : ScreenProfile.Pad;
        }


        public class State
        {
            private readonly LinkedList<PendingRecordBase> _pendingQueue;
            private readonly LinkedList<PageRecord> _uiStack;

            public State(LinkedList<PendingRecordBase> pendingQueue, LinkedList<PageRecord> uiStack)
            {
                _pendingQueue = pendingQueue;
                _uiStack = uiStack;
            }

            public int PopCount => GetStackPageCount<PopBase>();

            public int GetStackPageCount<T>() where T : ControllerBase
            {
                var count = 0;
                foreach (var item in _uiStack)
                    if (item.PageType == typeof(T))
                        count++;

                return count;
            }
        }

        private enum ScreenProfile
        {
            Mobile,

            Pad
            // Notch,
        }
    }
}