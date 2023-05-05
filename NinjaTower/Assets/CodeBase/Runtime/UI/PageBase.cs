using UnityEngine;
using UnityEngine.UI;

namespace Carotaa.Code
{
    [RequireComponent(typeof(Canvas))]
    [RequireComponent(typeof(GraphicRaycaster))]
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