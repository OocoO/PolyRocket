using PolyRocket.Game;
using UnityEngine;

namespace PolyRocket.UI
{
    public class UIPageBase: MonoBehaviour
    {
        public PrGlobal Global { get; private set; }
        
        public virtual void Init(PrGlobal global)
        {
            Global = global;
        }

        public void SetVisible(bool isVisible)
        {
            gameObject.SetActive(isVisible);
        }
    }
}