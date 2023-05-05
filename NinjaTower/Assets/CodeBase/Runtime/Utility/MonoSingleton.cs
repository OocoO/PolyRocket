using UnityEngine;

namespace Carotaa.Code
{
    public class MonoSingleton<T> : MonoBehaviour where T : MonoSingleton<T>
    {
        private static T _singleton;
        public static T Instance
        {
            get
            {
#if UNITY_EDITOR
                if (!Application.isPlaying)
                {
                    Debug.LogError("Please Enter Play Mode");
                    return null;
                }
#endif

                if (ReferenceEquals(_singleton, null))
                {
                    _singleton = (T) FindObjectOfType(typeof(T));

                    if (!_singleton)
                    {
                        var go = new GameObject($"MonoSingletons {typeof(T).Name}");

                        _singleton = go.AddComponent<T>();
                    }
                    
                    DontDestroyOnLoad(_singleton.gameObject);
                }

                return _singleton;
            }
        }

        public void WakeUp()
        {
            // do nothing
        }
    }
}