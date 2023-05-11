using System;
using Carotaa.Code;
using PolyRocket.Game;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace PolyRocket.UI
{
    [PageAddress("UI/HudPage")]
    public class PrPlayerHud : UIPage
    {
        public EmptyTouchPanel m_LeftButton;
        public EmptyTouchPanel m_RightButton;
        
        public TMP_Text BerryCount;

        private PrPlayer _player;

        public override void OnPush()
        {
            base.OnPush();

            _player = PushParam[0] as PrPlayer;
            // ReSharper disable once PossibleNullReferenceException
            _player.Level.BerryCount.Subscribe(OnBerryCountChange);
            OnBerryCountChange(_player.Level.BerryCount.Value);
        }

        public override void OnPop()
        {
            _player.Level.BerryCount.UnSubscribe(OnBerryCountChange);
        }

        private void OnBerryCountChange(int value)
        {
            BerryCount.text = $": {value}";
        }
    }
}