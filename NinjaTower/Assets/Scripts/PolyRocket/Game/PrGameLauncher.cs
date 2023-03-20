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

        public ShareEvent EPlayerMoveStart = ShareEvent.BuildEvent(nameof(EPlayerMoveStart));
        public ShareEvent EPlayerMoveEnd = ShareEvent.BuildEvent(nameof(EPlayerMoveEnd));
        public ShareEvent EPlayerMoveToTarget = ShareEvent.BuildEvent(nameof(EPlayerMoveToTarget));
        
        private bool _isGameStart;
        private int _moveStepRemain;

        public bool PlayerMoveLock => _moveStepRemain <= 0;

        private void Awake()
        {
            Init();
        }

        private void Init()
        {
            Physics2D.gravity = Vector2.zero;
            mainCamera = Camera.main;

            levelOne.gameObject.SetActive(false);
            levelTwo.gameObject.SetActive(false);

            EPlayerMoveStart.Subscribe(OnPlayerMoveStart);
            EPlayerMoveEnd.Subscribe(OnPlayerMoveEnd);
            EPlayerMoveToTarget.Subscribe(OnPlayerMoveToTarget);

            HidePop();
            
            JumpToLevel(levelOne);
        }

        public void JumpToLevel(PrGameLevel level)
        {
            if (_currentLevel) Destroy(_currentLevel.gameObject);

            var go = Instantiate(level.gameObject, transform);
            _currentLevel = go.GetComponent<PrGameLevel>();
            go.SetActive(true);
            
            // data init
            _moveStepRemain = _currentLevel.maxStepCount;
            _currentLevel.ball.Init(this);

            _isGameStart = true;
        }

        private void OnClickRestart()
        {
            // close ui
            HidePop();
            JumpToLevel(levelOne);
        }

        private void OnPlayerMoveStart()
        {
            if (_moveStepRemain <= 0)
            {
                throw new Exception("Invalid Move");
            }
            
            _moveStepRemain--;
        }

        private void OnPlayerMoveEnd()
        {
            if (_isGameStart)
            {
                // game over: failed
                _isGameStart = false;
                ShowGameOver();
            }
        }

        private void OnPlayerMoveToTarget()
        {
            if (_isGameStart)
            {
                _isGameStart = false;
                ShowGameSuccess();
            }
        }

        private void ShowGameOver()
        {
            popTitle.text = "Game Over";
            popBtnText.text = "Restart";

            var onClick = new Button.ButtonClickedEvent();
            onClick.AddListener(OnClickRestart);
            popBtn.onClick = onClick;
            
            uiPop.SetActive(true);
        }

        private void ShowGameSuccess()
        {
            popTitle.text = "Level Complete";
            popBtnText.text = "Next";

            var onClick = new Button.ButtonClickedEvent();
            onClick.AddListener(OnClickGotoNext);
            popBtn.onClick = onClick;
            
            uiPop.SetActive(true);
        }

        private void OnClickGotoNext()
        {
            JumpToLevel(levelTwo);
        }

        private void HidePop()
        {
            uiPop.SetActive(false);
        }
    }
}
