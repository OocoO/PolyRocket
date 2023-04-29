using UnityEngine;

namespace Carotaa.Code
{
    public abstract class PageBase : MonoBehaviour
    {
        public virtual void OnPush(object[] pushParam)
        {
        }

        public virtual void OnPop(object[] popParam)
        {
        }
    }
}