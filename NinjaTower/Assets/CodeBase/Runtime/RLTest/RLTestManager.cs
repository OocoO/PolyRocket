using System.Collections;
using System.Collections.Generic;
using Carotaa.Code;

namespace RLTest
{
    // ReSharper disable once InconsistentNaming
    public class RLTestManager : Singleton<RLTestManager>
    {
        protected override void OnCreate()
        {
            base.OnCreate();
        }

        public void Preform(IEnvironment env, IAgent agent, List<IReporter> reporters)
        {
            MonoHelper.Instance.StartCoroutine(GetTestCoroutine(env, agent, reporters));
        }

        private IEnumerator GetTestCoroutine(IEnvironment env, IAgent agent, List<IReporter> reporters)
        {
            yield return null;
        }

        public struct TestParameter
        {
            public int MaxLoops;

            public IEnvironment Env;
            public IAgent Agent;
            public List<IReporter> Reporters;
        }
    }
}