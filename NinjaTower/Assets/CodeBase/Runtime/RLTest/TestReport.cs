namespace RLTest
{
    public class TestReport
    {
        public enum Status
        {
            Success,
            Failed
        }

        public Status Code { get; }

        public virtual string Reason { get; }
    }
}