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
        
        public TMP_Text m_BerryCount;
        public TMP_Text m_LaunchTime;
        public TMP_Text m_Height;

        private PrPlayer _player;

        public override void OnPush()
        {
            base.OnPush();

            _player = PushParam[0] as PrPlayer;
            // ReSharper disable once PossibleNullReferenceException
            var level = _player.Level;
            level.BerryCount.Subscribe(OnBerryCountChange);
            level.LaunchTime.Subscribe(OnLunchTimeChange);
            level.Height.Subscribe(OnHeightChange);
            OnBerryCountChange(_player.Level.BerryCount.Value);
            OnLunchTimeChange(level.LaunchTime.Value);
            OnHeightChange(level.Height.Value);
        }

        public override void OnPop()
        {
            _player.Level.BerryCount.UnSubscribe(OnBerryCountChange);
            _player.Level.LaunchTime.UnSubscribe(OnLunchTimeChange);
            _player.Level.Height.UnSubscribe(OnHeightChange);
        }

        private void OnBerryCountChange(int value)
        {
            m_BerryCount.text = $": {value}";
        }
        
        private void OnLunchTimeChange(float value)
        {
            var time = TimeSpan.FromSeconds((int) value);
            m_LaunchTime.text = $"{time}";
        }
        
        private void OnHeightChange(float value)
        {
            m_Height.text = $"{value:F2} m";
        }
    }
}