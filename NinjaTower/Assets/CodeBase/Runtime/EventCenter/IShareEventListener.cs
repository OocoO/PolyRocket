namespace Carotaa.Code
{
    public interface IShareEventListener
    {
        void OnEventRaise();
    }

    public interface IShareEventListener<TData>
    {
        void OnEventRaise(TData data);
    }
}