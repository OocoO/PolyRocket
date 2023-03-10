namespace RLTest
{
    public interface IAgent
    {
        IAction Choice(IState state);
    }
}