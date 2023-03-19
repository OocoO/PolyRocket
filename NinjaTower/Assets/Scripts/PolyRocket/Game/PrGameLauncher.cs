using System;
using Carotaa.Code;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace PolyRocket.Game
{
    public class PrGameLauncher : MonoBehaviour
    {
        public PrGameLevel levelOne;
        public PrGameLevel levelTwo;

        public GameObject uiPop;
        public TMP_Text popTitle;
        public TMP_Text popBtnText;
        public Button popBtn;

        // public GraphicRaycaster gCaster;
        public Physics2DRaycaster pCaster;

        private PrGameLevel _currentLevel;
        
        public Camera mainCamera;

        public ShareEvent EPlayerMoveStart = ShareEvent.BuildEvent("EPlayerMoveStart");
        public ShareEvent EPlayerMoveEnd = ShareEvent.BuildEvent("EPlayerMoveEnd");

        private void Awake()
        {
            Init();
        }

        private void Init()
        {
            Physics2D.gravity = Vector2.zero;
            mainCamera = Camera.main;
            // gCaster.sortOrderPriority = 2;
            // pCaster.sortOrderPriority = 1;
            // pCaster.renderOrderPriority = 1;
            
            levelOne.gameObject.SetActive(false);
            levelTwo.gameObject.SetActive(false);

            EPlayerMoveEnd.Subscribe(OnPlayerMoveEnd);

            HidePop();
            
            JumpToLevel(levelOne);
        }

        public void JumpToLevel(PrGameLevel level)
        {
            if (_currentLevel) Destroy(_currentLevel.gameObject);

            var go = Instantiate(level.gameObject, transform);
            _currentLevel = go.GetComponent<PrGameLevel>();
            go.SetActive(true);
            _currentLevel.ball.Init(this);
        }

        private void OnRestartClick()
        {
            // close ui
            HidePop();
            JumpToLevel(levelOne);
        }

        private void OnPlayerMoveEnd()
        {
            ShowGameOver();
        }

        private void ShowGameOver()
        {
            popTitle.text = "Game Over";
            popBtnText.text = "Restart";

            var onClick = new Button.ButtonClickedEvent();
            onClick.AddListener(OnRestartClick);
            popBtn.onClick = onClick;
            
            uiPop.SetActive(true);
        }

        private void HidePop()
        {
            uiPop.SetActive(false);
        }
    }
}
