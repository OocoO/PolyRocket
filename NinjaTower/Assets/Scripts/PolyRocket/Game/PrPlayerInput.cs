using Carotaa.Code;
using PolyRocket.UI;
using UnityEngine.EventSystems;

namespace PolyRocket.Game
{
    public class PrPlayerInput
    {
        private readonly EmptyTouchPanel _panel;
        private readonly PrPlayer _player;
        
        public PrPlayerInput(PrPlayer player)
        {
            UIManager.Instance.Push<PrPlayerHud>();
            var hud = UIManager.Instance.Find<PrPlayerHud>();
            _panel = hud.m_touchPanel;
            _player = player;

            _panel.EPointerClick += OnClick;
        }

        private void OnClick(PointerEventData data)
        {
            _player.StateMachine.Driver.OnPointerClick?.Invoke(data);
        }

        public void OnDestroy()
        {
            UIManager.Instance.Pop<PrPlayerHud>();
        }
    }
}