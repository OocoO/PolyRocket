using Carotaa.Code;
using PolyRocket.UI;
using UnityEngine.EventSystems;

namespace PolyRocket.Game
{
    public class PrPlayerInput
    {
        private readonly PrPlayerHud _hud;
        private readonly PrPlayer _player;
        
        public PrPlayerInput(PrPlayer player)
        {
            UIManager.Instance.Push<PrPlayerHud>(player);
            _hud = UIManager.Instance.Find<PrPlayerHud>();
            _player = player;

            _hud.m_LeftButton.EPointerDown += OnPointerDownLeft;
            _hud.m_LeftButton.EPointerUp += OnPointerUpLeft;
            _hud.m_RightButton.EPointerDown += OnPointerDownRight;
            _hud.m_RightButton.EPointerUp += OnPointerUpRight;
        }

        private void OnPointerDownLeft(PointerEventData data)
        {
            _player.SideLeft.SetActive(true);
        }
        
        private void OnPointerUpLeft(PointerEventData data)
        {
            _player.SideLeft.SetActive(false);
        }
        
        private void OnPointerDownRight(PointerEventData data)
        {
            _player.SideRight.SetActive(true);
        }
        
        private void OnPointerUpRight(PointerEventData data)
        {
            _player.SideRight.SetActive(false);
        }
        

        public void OnDestroy()
        {
            UIManager.Instance.Pop(_hud);
        }
    }
}