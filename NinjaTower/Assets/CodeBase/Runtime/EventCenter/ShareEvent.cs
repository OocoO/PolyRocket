using System;
using System.Collections.Generic;

namespace Carotaa.Code
{
    public class ShareEvent : IGlobalVariable, IShareEvent
    {
        private readonly List<Action> _actions;
        private bool _isRaising;

        private readonly List<IShareEventListener> _listeners;

        protected ShareEvent()
        {
            _listeners = new List<IShareEventListener>();
            _actions = new List<Action>();
        }

        public string Name { get; set; }

        public void OnCreate()
        {
            // do something maybe
        }

        public void OnRelease()
        {
            // do something maybe
        }

        public bool Subscribe(IShareEventListener listener)
        {
            if (_listeners.Contains(listener)) return false;

            _listeners.Add(listener);
            return true;
        }

        public bool UnSubscribe(IShareEventListener listener)
        {
            if (!_listeners.Contains(listener)) return false;

            _listeners.Remove(listener);
            return true;
        }

        public bool Subscribe(Action action)
        {
            if (_actions.Contains(action)) return false;

            _actions.Add(action);
            return true;
        }

        public bool UnSubscribe(Action action)
        {
            if (!_actions.Contains(action)) return false;

            _actions.Remove(action);
            return true;
        }

        public void Raise(Predicate<object> predicate = null)
        {
            BeginRaise();
            Internal_Raise(predicate);
            EndRaise();
        }

        public static ShareEvent BuildEvent(string tag)
        {
            return new ShareEvent
            {
                Name = tag
            };
        }

        public static ShareEvent<T> BuildEvent<T>(string tag)
        {
            return new ShareEvent<T>
            {
                Name = tag
            };
        }

        public static ShareVariable<T> BuildVariable<T>(string tag)
        {
            return new ShareVariable<T>
            {
                Name = tag
            };
        }

        public bool UnSubscribe(Predicate<IShareEventListener> predicate)
        {
            var success = false;
            for (var i = _listeners.Count; i >= 0; i--)
            {
                var listener = _listeners[i];
                if (predicate(listener))
                {
                    _listeners.RemoveAt(i);
                    success = true;
                }
            }

            return success;
        }

        protected void Internal_Raise(Predicate<object> predicate)
        {
            var len = _actions.Count;
            for (var i = len - 1; i >= 0; i--)
            {
                var action = _actions[i];
                try
                {
                    if (predicate == null || predicate.Invoke(action)) action.Invoke();
                }
                catch (Exception e)
                {
                    EventTrack.LogError($"Invoke event {Name} failed with exception: {e}");
                }
            }

            len = _listeners.Count;
            for (var i = len - 1; i >= 0; i--)
            {
                var listener = _listeners[i];
                try
                {
                    if (predicate == null || predicate.Invoke(listener)) listener.OnEventRaise();
                }
                catch (Exception e)
                {
                    EventTrack.LogError($"Invoke event {Name} failed with exception: {e}");
                }
            }
        }

        public virtual void Clear()
        {
            _actions.Clear();
            _listeners.Clear();
        }

        protected void BeginRaise()
        {
#if DEBUG
            if (_isRaising) throw new Exception("Raising Event Recursive");

            _isRaising = true;
#endif
            EventTrack.LogTrace($"ShareEvent {Name} Begin");
        }

        protected void EndRaise()
        {
#if DEBUG
            _isRaising = false;
#endif
            EventTrack.LogTrace($"ShareEvent {Name} End");
        }
    }

    public class ShareEvent<TData> : ShareEvent, IShareEvent<TData>
    {
        private readonly List<Action<TData>> _actions;
        private readonly List<IShareEventListener<TData>> _listeners;

        public ShareEvent()
        {
            _listeners = new List<IShareEventListener<TData>>();
            _actions = new List<Action<TData>>();
        }

        public bool Subscribe(IShareEventListener<TData> listener)
        {
            if (_listeners.Contains(listener)) return false;

            _listeners.Add(listener);
            return true;
        }

        public bool UnSubscribe(IShareEventListener<TData> listener)
        {
            if (_listeners.Contains(listener)) return false;

            _listeners.Remove(listener);
            return true;
        }

        public bool Subscribe(Action<TData> action)
        {
            if (_actions.Contains(action)) return false;

            _actions.Add(action);
            return true;
        }

        public bool UnSubscribe(Action<TData> action)
        {
            if (!_actions.Contains(action)) return false;

            _actions.Remove(action);
            return true;
        }

        public void Raise(TData data, Predicate<object> predicate = null)
        {
            BeginRaise();
            base.Internal_Raise(predicate);
            Internal_Raise(data, predicate);
            EndRaise();
        }

        public Type GetDataType()
        {
            return typeof(TData);
        }

        public bool UnSubscribe(Predicate<IShareEventListener<TData>> predicate)
        {
            var success = false;
            for (var i = _listeners.Count; i >= 0; i--)
            {
                var listener = _listeners[i];
                if (predicate(listener))
                {
                    _listeners.RemoveAt(i);
                    success = true;
                }
            }

            return success;
        }

        protected void Internal_Raise(TData data, Predicate<object> predicate)
        {
            var len = _actions.Count;
            for (var i = len - 1; i >= 0; i--)
            {
                var action = _actions[i];
                try
                {
                    if (predicate == null || predicate.Invoke(action)) action.Invoke(data);
                }
                catch (Exception e)
                {
                    EventTrack.LogError($"Invoke event {Name} failed with exception: {e}");
                }
            }

            len = _listeners.Count;
            for (var i = len - 1; i >= 0; i--)
            {
                var listener = _listeners[i];
                try
                {
                    if (predicate == null || predicate.Invoke(listener)) listener.OnEventRaise(data);
                }
                catch (Exception e)
                {
                    EventTrack.LogError($"Invoke event {Name} failed with exception: {e}");
                }
            }
        }

        public override void Clear()
        {
            base.Clear();

            _actions.Clear();
            _listeners.Clear();
        }
    }
}