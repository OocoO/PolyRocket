using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

namespace Carotaa.Code
{
    public class AssetLoader
    {
        private static readonly Dictionary<string, AssetLoader> Loaders = new Dictionary<string, AssetLoader>();
        private static int _handleId = 10000;
        private readonly ShareEvent<Object> _onLoadComplete;
        private readonly HashSet<int> _refHandles;

        public readonly string Address;

        private AsyncOperationHandle _handle;

        private AssetLoader(string address)
        {
            Address = address;
            _refHandles = new HashSet<int>();
            _onLoadComplete = ShareEvent.BuildEvent<Object>($"AssetLoader.OnLoadComplete of {address}");
        }

        private int RefCount => _refHandles.Count;

        public Object Result
        {
            get
            {
                _handle.WaitForCompletion();
                return (Object) _handle.Result;
            }
        }

        [Conditional("UNITY_EDITOR")]
        public static void OnRuntimeInit()
        {
            Loaders.Clear();
        }

        // Support Event Pattern
        public static AssetHandle LoadAsyncWithCallback(string address, System.Action<Object> callback)
        {
            var handle = LoadAsync(address);
            handle.AddCallback(callback);
            return handle;
        }

        public static AssetHandle LoadAsync(string address)
        {
            if (!Loaders.ContainsKey(address))
            {
                // maybe: add some addition memory manage info
                var nLoader = new AssetLoader(address);
                Loaders.Add(address, nLoader);
            }

            var loader = Loaders[address];

            return loader.Internal_LoadAsync();
        }

        public static AssetLoader[] Internal_GetAll()
        {
            return Loaders.Values.ToArray();
        }

        public void AddCallback(int id, System.Action<Object> callback)
        {
            if (_handle.IsDone)
            {
                callback.Invoke(Result);
                return;
            }

            var listener = new AssetCallbackListener(id, callback);

            _onLoadComplete.Subscribe(listener);
        }


        public void Release(int id)
        {
            if (!_handle.IsValid()) return;

            if (_refHandles.Contains(id)) _refHandles.Remove(id);

            _onLoadComplete.UnSubscribe(x =>
            {
                if (x is AssetCallbackListener t) return t.ID == id;

                return false;
            });

            if (RefCount <= 0)
            {
                Addressables.Release(_handle);
                Loaders.Remove(Address);
            }
        }

        private AssetHandle Internal_LoadAsync()
        {
            if (!_handle.IsValid())
            {
                _handle = Addressables.LoadAssetAsync<Object>(Address);
                MonoHelper.Instance.StartCoroutine(WaitCallback());
            }

            var id = CreateId();
            _refHandles.Add(id);
            var handle = new AssetHandle(this, _handle, id);
            return handle;
        }

        private static int CreateId()
        {
            return _handleId++;
        }

        public override string ToString()
        {
            return $"Asset Load: {Address}";
        }

        private IEnumerator WaitCallback()
        {
            yield return _handle;
            _onLoadComplete.Raise(Result);
        }

        private class AssetCallbackListener : IShareEventListener<Object>
        {
            private readonly System.Action<Object> _callback;
            public readonly int ID;

            public AssetCallbackListener(int id, System.Action<Object> callback)
            {
                ID = id;
                _callback = callback;
            }

            public void OnEventRaise(Object data)
            {
                _callback(data);
            }
        }
    }

    // a handy wrapper of AsyncOperationHandle

    public class AssetHandle
    {
        private readonly int _id;
        private readonly AssetLoader _loader;
        private AsyncOperationHandle _handle;
        private bool _isReleased;

        public AssetHandle(AssetLoader loader, AsyncOperationHandle handle, int id)
        {
            _loader = loader;
            _handle = handle;
            _id = id;
        }

        public Object Result => _loader.Result;

        public void AddCallback(System.Action<Object> callback)
        {
            _loader.AddCallback(_id, callback);
        }

        // similar with Abort(), call this function in OnDestroy()
        public void Release()
        {
            if (_isReleased) return;

            _isReleased = true;

            _loader.Release(_id);
        }

        public void WaitForCompletion()
        {
            _handle.WaitForCompletion();
        }
    }
}