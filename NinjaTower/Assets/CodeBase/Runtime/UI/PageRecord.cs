using System;
using UnityEngine;
using UnityEngine.UI;
using Object = UnityEngine.Object;

namespace Carotaa.Code
{
    public class PageRecord
    {
        private readonly ControllerBase _controller;

        private readonly string _panelAddress;
        public readonly Type PageType;
        private AssetHandle _handle;

        private bool _isClickable;
        private GameObject _panel;
        private GraphicRaycaster[] _rayCasters;
        private int _sortOrder;
        private Transform _uiRoot;

        public PageRecord(Type type)
        {
            PageType = type;
            _controller = (ControllerBase) Activator.CreateInstance(PageType);
            _panelAddress = _controller.GetPanelAddress();
        }

        public bool IsVisible { get; private set; }

        public void SetClickable(bool isClickable)
        {
            if (_isClickable == isClickable) return;

            _isClickable = isClickable;
            Refresh();
        }

        public PageLayer GetLayer()
        {
            return _controller.Layer;
        }

        public void Init(Transform parent, int order)
        {
            _uiRoot = parent;
            _sortOrder = order;
        }

        public void OnCreate(PendingRecordBase record)
        {
            _handle = AssetLoader.LoadAsync(_panelAddress);

            Execute(_controller.OnPreload, record.CreateParam);
        }

        public void OnDestroy(PendingRecordBase record)
        {
            Execute(_controller.OnDeActive);
            Object.Destroy(_panel);
            _handle.Release();
        }

        private void CreatePanel()
        {
            _panel = (GameObject) Object.Instantiate(_handle.Result, _uiRoot);
            _rayCasters = _panel.GetComponentsInChildren<GraphicRaycaster>();

            // process Panel Order
            var panelCanvas = _panel.GetComponent<Canvas>();
            if (ReferenceEquals(panelCanvas, null)) panelCanvas = _panel.AddComponent<Canvas>();

            panelCanvas.overrideSorting = true;
            panelCanvas.sortingOrder = _sortOrder;
            panelCanvas.sortingLayerID = _controller.SortingLayerID();

            var childCanvas = _panel.GetComponentsInChildren<Canvas>();
            foreach (var canvas in childCanvas)
            {
                if (!ReferenceEquals(canvas, panelCanvas)) continue;

                if (canvas.overrideSorting) canvas.sortingOrder += _sortOrder;
            }
        }

        public void Show(PendingRecordBase record)
        {
            if (!_panel) CreatePanel();
            IsVisible = true;
            Refresh();
            Execute(_controller.OnActive);
        }

        public void Hide(PendingRecordBase record)
        {
            IsVisible = false;
            Refresh();
            Execute(_controller.OnDeActive);
        }

        private void Refresh()
        {
            _panel.SetActive(IsVisible);
            foreach (var raycaster in _rayCasters) raycaster.enabled = _isClickable;
        }

        private static void Execute(Action action)
        {
            try
            {
                action();
            }
            catch (Exception e)
            {
                EventTrack.LogError(e);
            }
        }

        private static void Execute(Action<object[]> action, object[] param)
        {
            try
            {
                action(param);
            }
            catch (Exception e)
            {
                EventTrack.LogError(e);
            }
        }
    }
}