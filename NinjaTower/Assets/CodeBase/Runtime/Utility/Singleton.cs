namespace Carotaa.Code
{
    public class Singleton<T> where T : Singleton<T>, new()
    {
        private static T _singleton;

        // ReSharper disable once StaticMemberInGenericType
        private static readonly object Lock = new object();

        public static T Instance
        {
            get
            {
                lock (Lock)
                {
                    if (_singleton == null)
                    {
                        _singleton = new T();
                        _singleton.OnCreate();
                    }

                    return _singleton;
                }
            }
        }

        protected virtual void OnCreate()
        {
        }

        public virtual void WakeUp()
        {
            // do nothing
        }
    }
}