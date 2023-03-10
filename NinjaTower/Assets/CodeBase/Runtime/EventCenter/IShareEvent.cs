using System;

namespace Carotaa.Code
{
    public interface IShareEvent
    {
        bool Subscribe(IShareEventListener listener);
        bool UnSubscribe(IShareEventListener listener);

        bool Subscribe(Action action);
        bool UnSubscribe(Action action);

        void Raise(Predicate<object> predicate);
    }

    public interface IShareEvent<TData> : IShareEvent
    {
        bool Subscribe(IShareEventListener<TData> listener);
        bool UnSubscribe(IShareEventListener<TData> listener);

        bool Subscribe(Action<TData> action);
        bool UnSubscribe(Action<TData> action);

        void Raise(TData data, Predicate<object> predicate);

        Type GetDataType();
    }
}