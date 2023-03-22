using System;
using Carotaa.Code;
using Cinemachine;
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

        public CinemachineVirtualCamera virtualCam;

        public int maxShotForce;
        public float speedDecrease;
        public float safeZoom; // the size of the ball: screen coord

        private PrGameLevel _levelPointer;
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

            _levelPointer = level;
            var go = Instantiate(level.gameObject, transform);
            _currentLevel = go.GetComponent<PrGameLevel>();
            go.SetActive(true);
            
            // data init
            _moveStepRemain = _currentLevel.maxStepCount;
            var worldToScreenMat = ScreenUtility.World2ScreenMatrix(mainCamera);
            safeZoom = worldToScreenMat.lossyScale.x * _currentLevel.ball.GetComponent<CircleCollider2D>().radius;

            var follow = _currentLevel.ball.transform;
            virtualCam.transform.position = follow.position;
            virtualCam.Follow = follow;
            
            _currentLevel.ball.Init(this);

            _isGameStart = true;
        }

        private void OnClickRestart()
        {
            // close ui
            HidePop();
            JumpToLevel(_levelPointer);
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
            if (_isGameStart && _moveStepRemain <= 0)
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
            HidePop();
            JumpToLevel(levelTwo);
        }

        private void HidePop()
        {
            uiPop.SetActive(false);
        }

        public void SetPhysicsPause(bool isPause)
        {
            Time.timeScale = isPause ? 0f : 1f;
        }
    }
}
