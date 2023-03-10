namespace Carotaa.Code
{
    /// <summary>
    ///     A Base Class for all ShareEvent and ShareVariable
    /// </summary>
    public interface INameable
    {
        string Name { get; set; }
    }

    public interface IGlobalVariable : INameable
    {
        void OnCreate();
        void OnRelease();
    }
}