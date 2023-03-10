using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using UnityEngine;

namespace Carotaa.Code
{
    public class Loom : MonoBehaviour
    {
        public static int maxThreads = 8;
        private static int numThreads;

        private static Loom _current;

        private static bool initialized;

        private readonly List<Action> _actions = new List<Action>();

        private readonly List<Action> _currentActions = new List<Action>();

        private readonly List<DelayedQueueItem> _currentDelayed = new List<DelayedQueueItem>();
        private readonly List<DelayedQueueItem> _delayed = new List<DelayedQueueItem>();
        private int _count;

        public static Loom Instance
        {
            get
            {
                Initialize();
                return _current;
            }
        }

        private void Awake()
        {
            _current = this;
            initialized = true;
        }

        // Update is called once per frame
        private void Update()
        {
            lock (_actions)
            {
                _currentActions.Clear();
                _currentActions.AddRange(_actions);
                _actions.Clear();
            }

            foreach (var a in _currentActions) Execute(a);
            lock (_delayed)
            {
                _currentDelayed.Clear();
                _currentDelayed.AddRange(_delayed.Where(d => d.time <= Time.time));
                foreach (var item in _currentDelayed)
                    _delayed.Remove(item);
            }

            foreach (var delayed in _currentDelayed) Execute(delayed.action);
        }


        private void OnDisable()
        {
            if (_current == this) _current = null;
        }

        private static void Initialize()
        {
            if (!initialized)
            {
                if (!Application.isPlaying)
                    return;
                initialized = true;
                var g = new GameObject("Loom");
                DontDestroyOnLoad(g);
                _current = g.AddComponent<Loom>();
            }
        }

        public static void QueueOnMainThread(Action action)
        {
            QueueOnMainThread(action, 0f);
        }

        public static void QueueOnMainThread(Action action, float time)
        {
            if (time != 0)
                lock (Instance._delayed)
                {
                    Instance._delayed.Add(new DelayedQueueItem {time = Time.time + time, action = action});
                }
            else
                lock (Instance._actions)
                {
                    Instance._actions.Add(action);
                }
        }

        public static Thread RunAsync(Action a)
        {
            Initialize();
            while (numThreads >= maxThreads) Thread.Sleep(1);
            Interlocked.Increment(ref numThreads);
            ThreadPool.QueueUserWorkItem(RunAction, a);
            return null;
        }

        private static void RunAction(object action)
        {
            try
            {
                ((Action) action)();
            }
            catch (Exception e)
            {
                EventTrack.LogError(e);
            }
            finally
            {
                Interlocked.Decrement(ref numThreads);
            }
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

        private struct DelayedQueueItem
        {
            public float time;
            public Action action;
        }
    }
}