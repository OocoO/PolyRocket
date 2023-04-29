using System;
using Carotaa.Code;
using PolyRocket.Game;
using TMPro;
using UnityEngine;

namespace PolyRocket.UI
{
    [PageAddress("UI/PrPlayerHud")]
    public class PrPlayerHud : PageBase
    {
        public TMP_Text dashCount;

        private PrGlobal _global;

        public override void OnPush(object[] pushParam)
        {
            base.OnPush(pushParam);

            _global = pushParam[0] as PrGlobal;
            // ReSharper disable once PossibleNullReferenceException
            OnDashCountChange(_global.VPlayerDashLevel.Value);
            _global.VPlayerDashLevel.Subscribe(OnDashCountChange);
        }

        public override void OnPop(object[] popParam)
        {
            base.OnPop(popParam);
            
            _global.VPlayerDashLevel.UnSubscribe(OnDashCountChange);
        }


        private void OnDashCountChange(int level)
        {
            dashCount.text = $"Dash Level: {level}";
        }
    }
}