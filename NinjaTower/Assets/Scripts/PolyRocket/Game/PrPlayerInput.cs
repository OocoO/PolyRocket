using Carotaa.Code;
using PolyRocket.UI;
using UnityEngine;
using UnityEngine.EventSystems;

namespace PolyRocket.Game
{
    public class PrPlayerInput
    {
        private readonly PrPlayerHud _hud;
        private readonly PrPlayer _player;

        private bool _isLeftBtnDown;
        private bool _isRightBtnDown;
        
        private enum ActiveModule
        {
            None,
            Left,
            Right,
            Main
        }
        
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
            SetLeftButtonDown(true);
        }

        private void SetLeftButtonDown(bool isDown)
        {
            _isLeftBtnDown = isDown;
        }

        private void OnPointerUpLeft(PointerEventData data)
        {
            SetLeftButtonDown(false);
        }
        
        private void OnPointerDownRight(PointerEventData data)
        {
            SetRightButtonDown(true);
        }

        private void SetRightButtonDown(bool isDown)
        {
            _isRightBtnDown = isDown;
        }

        private void OnPointerUpRight(PointerEventData data)
        {
            SetRightButtonDown(false);
        }

        public void Update()
        {
            var leftDown = Input.GetKeyDown(KeyCode.LeftArrow);
            var rightDown = Input.GetKeyDown(KeyCode.RightArrow);

            var leftUp = Input.GetKeyUp(KeyCode.LeftArrow);
            var rightUp = Input.GetKeyUp(KeyCode.RightArrow);
            
            if (leftDown) SetLeftButtonDown(true);
            if (rightDown) SetRightButtonDown(true);
            
            if (leftUp) SetLeftButtonDown(false);
            if (rightUp) SetRightButtonDown(false);

            var module = ActiveModule.None;
            if (_isLeftBtnDown && !_isRightBtnDown)
            {
                module = ActiveModule.Left;
            }
            else if (!_isLeftBtnDown && _isRightBtnDown)
            {
                module = ActiveModule.Right;
            }
            else if (_isLeftBtnDown && _isRightBtnDown)
            {
                module = ActiveModule.Main;
            }
            
            _player.SideLeft.SetActive(module == ActiveModule.Left);
            _player.SideRight.SetActive(module == ActiveModule.Right);
            _player.Main.SetActive(module == ActiveModule.Main);
        }
        

        public void OnDestroy()
        {
            UIManager.Instance.Pop(_hud);
        }
    }
}