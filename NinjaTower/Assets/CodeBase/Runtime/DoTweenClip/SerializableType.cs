using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Object = UnityEngine.Object;

namespace Carotaa.Code
{
    [Serializable]
    public class SerializableType: ISerializationCallbackReceiver
    {
        [SerializeField] private int m_Value;

        private Type _type;
        public Type Value
        {
            get => _type;
            set
            {
                var table = GetType2Int();
                if (!table.ContainsKey(value))
                {
                    throw new Exception($"Undefined type: {value}");
                }

                _type = value;
            }
        }

        public void OnBeforeSerialize()
        {
            var table = GetType2Int();
            m_Value = table[Value];
        }

        public void OnAfterDeserialize()
        {
            var table = GetInt2Type();
            Value = table[m_Value];
        }


        private static Dictionary<int, Type> _int2Type;

        private static Dictionary<int, Type> GetInt2Type()
        {
            if (_int2Type == null)
            {
                // define your custom types - hash in this dic
                _int2Type = new Dictionary<int, Type>()
                {
                    {0, typeof(Object)},
                    {1, typeof(GameObject)},
                    {2, typeof(RectTransform)},
                    {3, typeof(Transform)},
                    {4, typeof(Image)},
                    {5, typeof(CanvasGroup)},
                    {6, typeof(Text)},
                    {7, typeof(ParticleSystem)},
                    {8, typeof(Button)},
                    {9, typeof(Canvas)},
                    {10, typeof(GraphicRaycaster)},
                };
            }

            return _int2Type;
        }

        private static Dictionary<Type, int> _type2Int;
        private static Dictionary<Type, int> GetType2Int()
        {
            if (_type2Int == null)
            {
                var hash = GetInt2Type();
                _type2Int = new Dictionary<Type, int>();
                foreach (var kvp in hash)
                {
                    _type2Int[kvp.Value] = kvp.Key;
                }
            }

            return _type2Int;
        }
    }
}