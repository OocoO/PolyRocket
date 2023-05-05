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
            var mainCam = PrCameraManager.Instance.m_MainCam;
            var clickWorldPos = mainCam.ScreenToWorldPoint(data.position);

            var playerX = _player.transform.position.x;
            if (playerX > clickWorldPos.x)
            {
                _player.MoveLeft();
            }
            else
            {
                _player.MoveRight();
            }
        }

        public void OnDestroy()
        {
            UIManager.Instance.Pop<PrPlayerHud>();
        }
    }
}