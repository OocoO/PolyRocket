#if DEBUG
#define GLOBAL_VARIABLE_TYPE_CHECK
#endif

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace Carotaa.Code
{
    public static class GlobalVariable
    {
        private static readonly Dictionary<string, RefObject> _buffer = new Dictionary<string, RefObject>();

        [Conditional("UNITY_EDITOR")]
        public static void OnRuntimeInit()
        {
            _buffer.Clear();
        }

        // TODO: Add some default Get, Release function

        public static void Internal_GetAll(List<RefObject> list)
        {
            list?.AddRange(_buffer.Values);
        }

        public static RefObject Internal_Get(string key, Type type, Func<IGlobalVariable> factory)
        {
            // disable type check when release
#if GLOBAL_VARIABLE_TYPE_CHECK
            if (!type.GetInterfaces().Contains(typeof(IGlobalVariable))) throw new NameableExpectedException(key, type);
#endif

            if (_buffer.TryGetValue(key, out var obj))
            {
                obj = _buffer[key];
#if GLOBAL_VARIABLE_TYPE_CHECK
                var oType = obj.Value.GetType();
                if (oType != type && !oType.IsSubclassOf(type)) throw new TypeNotMatchException(key, obj.Value, type);
#endif
            }
            else
            {
                var item = factory.Invoke();
                item.Name = key;
                item.OnCreate();

                obj = new RefObject
                {
                    RefCount = 0,
                    Value = item
                };

                _buffer.Add(key, obj);
            }

            return obj;
        }

        public static void Internal_RefIncrease(RefObject refObj)
        {
            var key = refObj.Value.Name;
            if (!_buffer.ContainsKey(key)) return;

            refObj.RefCount++;
        }

        public static void Internal_RefDecrease(RefObject refObj)
        {
            var key = refObj.Value.Name;
            if (!_buffer.ContainsKey(key)) return;

            refObj.RefCount--;

            if (refObj.RefCount <= 0)
            {
                refObj.Value.OnRelease();
                _buffer.Remove(key);
            }
        }

        public class RefObject
        {
            public int RefCount;
            public IGlobalVariable Value;
        }

#if GLOBAL_VARIABLE_TYPE_CHECK
        private class TypeNotMatchException : Exception
        {
            public TypeNotMatchException(string key, IGlobalVariable nameable, Type expect)
                : base(
                    $"Key: {key} is already used by a variable {nameable} with type {nameable.GetType()}, unable to create a another type {expect}")
            {
            }
        }

        private class NameableExpectedException : Exception
        {
            public NameableExpectedException(string key, Type expect)
                : base($"Unable to create a Type {expect} with key: {key}, Type Must imply interface : IGlobalVariable")
            {
            }
        }
#endif
    }
}