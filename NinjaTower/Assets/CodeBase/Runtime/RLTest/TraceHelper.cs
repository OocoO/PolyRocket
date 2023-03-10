using System.Collections.Generic;
using Carotaa.Code;

namespace RLTest
{
    public class TraceHelper : Singleton<TraceHelper>
    {
        private List<Trace> _list;

        protected override void OnCreate()
        {
            _list = new List<Trace>();
        }

        public void Log(IState state, IAction action, float reward)
        {
            _list.Add(new Trace
            {
                State = state,
                Action = action,
                Reward = reward
            });
        }

        public List<Trace> GetTrace()
        {
            return _list;
        }

        public struct Trace
        {
            public IState State;
            public IAction Action;
            public float Reward;
        }
    }
}