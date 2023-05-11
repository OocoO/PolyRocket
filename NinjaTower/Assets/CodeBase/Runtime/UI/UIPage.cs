using UnityEngine;
using UnityEngine.UI;

namespace Carotaa.Code
{
    [RequireComponent(typeof(Canvas))]
    [RequireComponent(typeof(GraphicRaycaster))]
    public abstract class UIPage : MonoBehaviour
    {
        public object[] PushParam;
        
        public virtual void OnPush()
        {
        }

        public virtual void OnPop()
        {
        }
    }
}