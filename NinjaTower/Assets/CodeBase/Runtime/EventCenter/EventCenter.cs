using System;

namespace Carotaa.Code
{
    public static class EventCenter
    {
        public static void Subscribe(string eventKey, Action onEventRaise)
        {
            var refObj = GlobalVariable.Internal_Get(eventKey,
                typeof(ShareEvent),
                () => ShareEvent.BuildEvent(eventKey));

            var success = ((ShareEvent) refObj.Value).Subscribe(onEventRaise);

            if (success) GlobalVariable.Internal_RefIncrease(refObj);
        }

        public static void UnSubscribe(string eventKey, Action onEventRaise)
        {
            var refObj = GlobalVariable.Internal_Get(eventKey,
                typeof(ShareEvent),
                () => ShareEvent.BuildEvent(eventKey));

            var success = ((ShareEvent) refObj.Value).UnSubscribe(onEventRaise);

            if (success) GlobalVariable.Internal_RefDecrease(refObj);
        }

        public static void Subscribe<TData>(string eventKey, Action<TData> onEventRaise)
        {
            var refObj = GlobalVariable.Internal_Get(eventKey,
                typeof(ShareEvent<TData>),
                () => ShareEvent.BuildEvent<TData>(eventKey));

            var success = ((ShareEvent<TData>) refObj.Value).Subscribe(onEventRaise);

            if (success) GlobalVariable.Internal_RefIncrease(refObj);
        }

        public static void UnSubscribe<TData>(string eventKey, Action<TData> onEventRaise)
        {
            var refObj = GlobalVariable.Internal_Get(eventKey,
                typeof(ShareEvent<TData>),
                () => ShareEvent.BuildEvent<TData>(eventKey));
            var success = ((ShareEvent<TData>) refObj.Value).UnSubscribe(onEventRaise);

            if (success) GlobalVariable.Internal_RefDecrease(refObj);
        }

        public static void Raise(string eventKey)
        {
            var refObj = GlobalVariable.Internal_Get(eventKey,
                typeof(ShareEvent),
                () => ShareEvent.BuildEvent(eventKey));
            var e = (ShareEvent) refObj.Value;
            e.Raise();
        }

        public static void Raise<TData>(string eventKey, TData data)
        {
            var refObj = GlobalVariable.Internal_Get(eventKey,
                typeof(ShareEvent<TData>),
                () => ShareEvent.BuildEvent<TData>(eventKey));
            var e = (ShareEvent<TData>) refObj.Value;
            e.Raise(data);
        }
    }
}