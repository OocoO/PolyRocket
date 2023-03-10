namespace RLTest
{
    public interface IReporter
    {
        TestReport Test(IState state);
    }
}