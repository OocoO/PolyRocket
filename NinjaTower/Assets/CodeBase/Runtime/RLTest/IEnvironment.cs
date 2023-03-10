using System.Collections.Generic;

namespace RLTest
{
    public interface IEnvironment
    {
        IState GetState();
        List<IAction> GetAction();

        void Update(IAction action);
    }
}