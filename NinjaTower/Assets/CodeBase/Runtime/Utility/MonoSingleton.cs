using UnityEngine;

namespace Carotaa.Code
{
    public abstract class MonoSingletonBase : MonoBehaviour
    {
        protected static GameObject Go;

        protected static bool IsExiting;

        protected virtual void OnApplicationQuit()
        {
            IsExiting = true;
        }
    }

    public class MonoSingleton<T> : MonoSingletonBase where T : MonoSingleton<T>
    {
        private static T _singleton;

        // ReSharper disable once StaticMemberInGenericType
        private static readonly object Lock = new object();

        public static T Instance
        {
            get
            {
                lock (Lock)
                {
                    if (IsExiting) return _singleton;

#if UNITY_EDITOR
                    if (!Application.isPlaying)
                    {
                        Debug.LogError("Please Enter Play Mode");
                        return null;
                    }
#endif

                    if (!_singleton)
                    {
                        _singleton = (T) FindObjectOfType(typeof(T));

                        if (!_singleton)
                        {
                            if (!Go)
                            {
                                Go = new GameObject("MonoSingletons");
                                DontDestroyOnLoad(Go);
                            }

                            _singleton = Go.AddComponent<T>();
                        }

                        _singleton.OnCreate();
                    }

                    return _singleton;
                }
            }
        }

        protected virtual void OnCreate()
        {
        }

        public void WakeUp()
        {
            // do nothing
        }
    }
}