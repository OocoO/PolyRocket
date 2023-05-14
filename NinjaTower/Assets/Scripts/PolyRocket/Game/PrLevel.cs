using System;
using System.Collections.Generic;
using System.Reflection;
using Carotaa.Code;
using PolyRocket.Game.Actor;
using PolyRocket.SO;
using UnityEngine;

namespace PolyRocket.Game
{
    public class PrLevel : MonoBehaviour
    {
        public int version;
        
        public const float MaxCameraSize = 24f;

        public PrPlayer m_Player;
        public Camera m_LevelCamera;
        public LevelConfig Config;

        public readonly ShareVariable<int> BerryCount = new ShareVariable<int>();
        public readonly ShareVariable<float> LaunchTime = new ShareVariable<float>();
        public readonly ShareVariable<float> Height = new ShareVariable<float>();

        [NonSerialized] public List<PrActor> Actors = new List<PrActor>();

        public float MainForce
        {
            get
            {
                return Config.m_MainForce + BerryCount.Value * Config.m_ForcePerBerry;
            }
        }
        public float Gravity => Physics2D.gravity.y * Config.GravityScale;

        // in theory, this is the max speed player can reach
        public float MaxPlayerSpeed
        {
            get
            {
                var deltaTime = Time.fixedDeltaTime;
                return (MainForce + Gravity) * deltaTime / (1 - Config.SpeedDcc);
            }
        }

        public float RefTime
        {
            get
            {
                return Config.m_CameraRefTime * (1 + BerryCount.Value * 5f);
            }
        }

        public float MaxCameraSpeed
        {
            get
            {
                return MaxPlayerSpeed * Config.m_MaxCameraSpeedScale;
            }
        }

        public void OnPush()
        {
            LaunchTime.Value = 0f;
            Height.Value = 0f;
            Actors = new List<PrActor>();

            PrCameraManager.Instance.AddWorldCamera(m_LevelCamera);
        }
        
        public void OnPop()
        {
            PrCameraManager.Instance.RemoveWorldCamera(m_LevelCamera);
        }
    }

    public class PrLevelInfo
    {
        private static PrLevelInfo _levelOne = new PrLevelInfo(1, "LevelOne");
        private static PrLevelInfo _levelTwo = new PrLevelInfo(2, "LevelTwo");
        
        public readonly int Index;
        public readonly string Name;

        private PrLevelInfo(int index, string name)
        {
            Name = name;
            Index = index;
        }
        public PrLevel GetLevel()
        {
            var go = Resources.Load<GameObject>($"Prefabs/Levels/{Name}");
            return go.GetComponent<PrLevel>();
        }

        private static Dictionary<int, PrLevelInfo> _cache;
        public static PrLevelInfo Find(int id)
        {
            Init();
            _cache.TryGetValue(id, out var value);

            return value;
        }

        public static List<PrLevelInfo> GetAll()
        {
            Init();

            var list = new List<PrLevelInfo>(_cache.Values);
            list.Sort((a, b) => a.Index.CompareTo(b.Index));

            return list;
        }

        public void JumpToLevel()
        {
            PrLevelManager.Instance.JumpToLevel(this);
        }

        private static void Init()
        {
            if (_cache == null)
            {
                // factory
                _cache = new Dictionary<int, PrLevelInfo>();
                var fieldInfos = typeof(PrLevelInfo).GetFields(BindingFlags.Static | BindingFlags.NonPublic);
                foreach (var fieldInfo in fieldInfos)
                {
                    var info = fieldInfo.GetValue(null);
                    if (info is PrLevelInfo pInfo)
                    {
                        _cache[pInfo.Index] = pInfo;
                    }
                }
            }
        }
    }
}