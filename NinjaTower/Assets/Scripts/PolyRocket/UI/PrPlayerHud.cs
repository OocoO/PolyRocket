using System;
using PolyRocket.Game;
using TMPro;
using UnityEngine;

namespace PolyRocket.UI
{
    public class PrPlayerHud : UIPageBase
    {
        public TMP_Text dashCount;

        public override void Init(PrGlobal global)
        {
            base.Init(global);
            
            OnDashCountChange(global.VPlayerDashLevel.Value);
            global.VPlayerDashLevel.Subscribe(OnDashCountChange);
        }


        private void OnDestroy()
        {
            Global.VPlayerDashLevel.UnSubscribe(OnDashCountChange);
        }

        private void OnDashCountChange(int level)
        {
            dashCount.text = $"Dash Level: {level}";
        }
    }
}