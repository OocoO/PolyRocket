namespace Carotaa.Code
{
    public class Singleton<T> where T : Singleton<T>, new()
    {
        private static T _singleton;
        
        public static T Instance
        {
            get
            {
                if (_singleton == null)
                {
                    _singleton = new T();
                    _singleton.OnCreate();
                }

                return _singleton;
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