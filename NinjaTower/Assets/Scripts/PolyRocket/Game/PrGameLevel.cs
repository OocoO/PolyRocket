using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

namespace PolyRocket.Game
{
    public class PrGameLevel : MonoBehaviour
    {
        public int version;

        public PrBall ball;

        public int maxStepCount;
    }

    public class PrGameLevelInfo
    {
        private static PrGameLevelInfo _levelOne = new PrGameLevelInfo(1, "LevelOne");
        private static PrGameLevelInfo _levelTwo = new PrGameLevelInfo(2, "LevelTwo");


        public readonly int Index;
        public readonly string Name;

        public PrGameLevelInfo(int index, string name)
        {
            Name = name;
            Index = index;
        }
        public PrGameLevel GetLevel()
        {
            var go = Resources.Load<GameObject>($"Prefabs/Levels/{Name}");
            return go.GetComponent<PrGameLevel>();
        }

        private static Dictionary<int, PrGameLevelInfo> _cache;
        public static PrGameLevelInfo Find(int id)
        {
            Init();
            _cache.TryGetValue(id, out var value);

            return value;
        }

        public static List<PrGameLevelInfo> GetAll()
        {
            Init();

            var list = new List<PrGameLevelInfo>(_cache.Values);
            list.Sort((a, b) => a.Index.CompareTo(b.Index));

            return list;
        }

        private static void Init()
        {
            if (_cache == null)
            {
                // factory
                _cache = new Dictionary<int, PrGameLevelInfo>();
                var fieldInfos = typeof(PrGameLevelInfo).GetFields(BindingFlags.Static | BindingFlags.NonPublic);
                foreach (var fieldInfo in fieldInfos)
                {
                    var info = fieldInfo.GetValue(null);
                    if (info is PrGameLevelInfo pInfo)
                    {
                        _cache[pInfo.Index] = pInfo;
                    }
                }
            }
        }
    }
}