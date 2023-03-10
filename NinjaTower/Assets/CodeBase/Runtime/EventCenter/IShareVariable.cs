namespace Carotaa.Code
{
    public interface IShareVariableBase
    {
        // used for grammar constrain
    }

    public interface IShareVariable<TData> : IShareEvent<TData>, IShareVariableBase, INameable
    {
        TData Value { get; set; }
    }
}