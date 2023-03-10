// a rect transform preset extenstion class, add this component to gb to store multiple rectTransform preset

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using UnityEditor;
using UnityEngine;

// a auto preset saver
namespace Carotaa.Code
{
    [ExecuteAlways]
    [RequireComponent(typeof(RectTransform))]
    [DisallowMultipleComponent]
    // ReSharper disable once InconsistentNaming
    public class MultiScreenRect : MonoBehaviour, ISerializationCallbackReceiver
    {
        public enum Device
        {
            /*
             * Note: for save file reason, plz add a new enum value to keep all value saved
             */
            Mobile = 0,
            Pad = 1,

            Notch = 2
            // MobileHorizontal = 2,
        }

        private static readonly List<RectSaveData> DataWorkList = new List<RectSaveData>();
        private static readonly List<MultiScreenRect> PresetWorkList = new List<MultiScreenRect>();

        // Set all your design size here, used to support normalised runtime rect size
        private static readonly Dictionary<Device, Vector2> s_DesignSize = new Dictionary<Device, Vector2>
        {
            {Device.Mobile, new Vector2(1080, 1920)},
            {Device.Pad, new Vector2(1536, 2048)},
            {Device.Notch, new Vector2(1080, 2340)}
        };

        // set this to change current preset
        private static readonly ShareVariable<Device> RectProfile =
            ShareEvent.BuildVariable<Device>("MFMultiScreen.RectProfile");

        [SerializeField] private Device m_ProfileType;
        [SerializeField] private bool m_IsNormalizeHorizontal;
        [SerializeField] private bool m_IsNormalizeVertical;
        [SerializeField] private RectSaveData[] m_Datas;
        private readonly Dictionary<Device, RectSaveData> _buffer = new Dictionary<Device, RectSaveData>();
        private bool _afterAwake;
        [NonSerialized] private bool _isBuildingRect;
        private bool _isOverride;

        private RectSaveData _overrideData;

        private RectTransform rectTransform => transform as RectTransform;

        private void Awake()
        {
            if (Application.isPlaying)
            {
                if (_isOverride)
                    SetRectSize(rectTransform, _overrideData);
                else
                    UpdateRectTransform(RectProfile.Value);
            }

            AddListener();
            _afterAwake = true;
        }


        private void OnDestroy()
        {
            RemoveListener();
        }

        public void OnBeforeSerialize()
        {
            DataWorkList.Clear();

            foreach (var i in Enum.GetValues(typeof(Device)).Cast<Device>())
            {
                var data = RectSaveData.From(rectTransform);
                if (_buffer.ContainsKey(i))
                    data = _buffer[i];
                else
                    _buffer.Add(i, data);
                DataWorkList.Add(data);
            }

            m_Datas = DataWorkList.ToArray();
        }

        public void OnAfterDeserialize()
        {
            _buffer.Clear();
            foreach (var i in Enum.GetValues(typeof(Device)).Cast<Device>())
            {
                var index = (int) i;
                if (index >= m_Datas.Length) index = 0;

                var data = m_Datas.Length == 0 ? RectSaveData.From(rectTransform) : m_Datas[index];
                _buffer.Add(i, data);
            }
        }

        public static void SetRectProfileValue(Device value)
        {
            RectProfile.Value = value;
        }

        public static Device ReadRectProfileValue()
        {
            return RectProfile.Value;
        }

        public void Refresh()
        {
            UpdateRectTransform(RectProfile.Value);
        }

        public RectSaveData ReadData(Device type)
        {
            return _buffer[type];
        }

        public void OverrideRect(RectSaveData data)
        {
            _overrideData = data;
            _isOverride = true;

            if (_afterAwake) SetRectSize(rectTransform, data);
        }

        private void UpdateRectTransform(Device device)
        {
            var data = _buffer[device];
            var designSize = s_DesignSize[device];
            var screenSize = GetRootCanvasSize(rectTransform);
            SetRectSizeExt(rectTransform,
                data,
                designSize,
                screenSize,
                m_IsNormalizeHorizontal,
                m_IsNormalizeVertical);
        }

        // only some value changed in editor mode
        private void EditorUpdate()
        {
            if (Application.isPlaying) return;
            if (_isBuildingRect) return;
            if (!transform.hasChanged) return;

            var screenSize = GetRootCanvasSize(rectTransform);
            var designSize = s_DesignSize[m_ProfileType];
            if (screenSize != designSize)
                // Debug.Log($"Unable to save profile: {m_ProfileType} under screen size : {screenSize}");
                return;
            var data = _buffer[m_ProfileType];
            data.Build(rectTransform);
            _buffer[m_ProfileType] = data;

            transform.hasChanged = false;
        }

        [Conditional("UNITY_EDITOR")]
        private void AddListener()
        {
#if UNITY_EDITOR
            if (!Application.isPlaying) EditorApplication.update += EditorUpdate;
#endif
        }

        [Conditional("UNITY_EDITOR")]
        private void RemoveListener()
        {
#if UNITY_EDITOR
            if (!Application.isPlaying) EditorApplication.update -= EditorUpdate;
#endif
        }


        private static void SetRectSize(RectTransform rect, RectSaveData data)
        {
            SetRectSizeExt(rect,
                data,
                Vector2.one,
                Vector2.one,
                false,
                false);
        }

        private static void SetRectSizeExt(
            RectTransform target,
            RectSaveData data,
            Vector2 designSize,
            Vector2 screenSize,
            bool isHoriNormalized,
            bool isVertNormalized)
        {
            data.Set(target);
            var size = data.m_DesignRect.size;
            if (isHoriNormalized)
                target.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal,
                    size.x / designSize.x * screenSize.x);

            if (isVertNormalized)
                target.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical,
                    size.y / designSize.y * screenSize.y);
        }

        // in-case root canvas is resized by canvas scaler
        private static Vector2 GetRootCanvasSize(RectTransform transform)
        {
            var root = transform.root as RectTransform;
            if (ReferenceEquals(root, null)) return ScreenUtility.Size;

            return root.rect.size;
        }

        [Serializable]
        public struct RectSaveData
        {
            [SerializeField] public Vector3 m_Position;
            [SerializeField] public Vector2 m_SizeDelta;
            [SerializeField] public Vector2 m_AnchorMin;
            [SerializeField] public Vector2 m_AnchorMax;
            [SerializeField] public Vector2 m_Pivot;
            [SerializeField] public Rect m_DesignRect; // some helper info

            public static RectSaveData From(RectTransform rect)
            {
                var data = new RectSaveData();
                data.Build(rect);
                return data;
            }

            public void Build(RectTransform rect)
            {
                Build(rect, ref this);
            }

            public void Set(RectTransform rect)
            {
                Set(rect, this);
            }

            public static void Set(RectTransform rect, RectSaveData data)
            {
                rect.pivot = data.m_Pivot;
                rect.anchorMin = data.m_AnchorMin;
                rect.anchorMax = data.m_AnchorMax;
                rect.anchoredPosition3D = data.m_Position;
                rect.sizeDelta = data.m_SizeDelta;
            }

            public static void Build(RectTransform rect, ref RectSaveData data)
            {
                data.m_Position = rect.anchoredPosition3D;
                data.m_SizeDelta = rect.sizeDelta;
                data.m_AnchorMin = rect.anchorMin;
                data.m_AnchorMax = rect.anchorMax;
                data.m_Pivot = rect.pivot;
                data.m_DesignRect = rect.rect;
            }
        }


#if UNITY_EDITOR
        public void SetProfileEditor(Device type)
        {
            // update the entire hierarchy
            var root = rectTransform.root;
            PresetWorkList.Clear();
            root.GetComponentsInChildren(PresetWorkList);

            foreach (var child in PresetWorkList) child.SetProfile(type);
        }

        private void SetProfile(Device type)
        {
            _isBuildingRect = true;

            m_ProfileType = type;
            var data = _buffer[type];
            data.Set(rectTransform);

            _isBuildingRect = false;
        }
#endif
    }
}