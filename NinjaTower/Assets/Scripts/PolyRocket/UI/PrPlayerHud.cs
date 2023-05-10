using System;
using Carotaa.Code;
using PolyRocket.Game;
using TMPro;
using UnityEngine;

namespace PolyRocket.UI
{
    [PageAddress("UI/HudPage")]
    public class PrPlayerHud : UIPage
    {
        public EmptyTouchPanel m_touchPanel;
        public TMP_Text BerryCount;

        private PrPlayer _player;

        public override void OnPush(object[] pushParam)
        {
            base.OnPush(pushParam);

            _player = pushParam[0] as PrPlayer;
            // ReSharper disable once PossibleNullReferenceException
            _player.Level.BerryCount.Subscribe(OnBerryCountChange);
            OnBerryCountChange(_player.Level.BerryCount.Value);
        }

        public override void OnPop(object[] popParam)
        {
            _player.Level.BerryCount.UnSubscribe(OnBerryCountChange);
        }

        private void OnBerryCountChange(int value)
        {
            BerryCount.text = $": {value}";
        }
    }
}