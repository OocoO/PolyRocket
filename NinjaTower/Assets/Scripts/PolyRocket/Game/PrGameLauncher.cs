using System;
using System.Collections.Generic;
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
        public GameObject uiPop;
        public TMP_Text popTitle;
        public TMP_Text popBtnText;
        public Button popBtn;

        public CinemachineBrain camBrain;
        public CinemachineVirtualCamera virtualCam;
        public DragPanel camDragPanel;
        public Transform camTarget;

        public int maxShotForce;
        public float speedDecrease;
        public float safeZoom; // the size of the ball: screen coord

        private List<PrGameLevelInfo> _levels;
        private PrGameLevelInfo _currentLevelInfo;
        private PrGameLevel _currentLevel;
        private int _currentIndex;

        private bool _cameraFollow;
        
        public Camera mainCamera;

        public ShareEvent EPlayerMoveStart = ShareEvent.BuildEvent(nameof(EPlayerMoveStart));
        public ShareEvent EPlayerMoveEnd = ShareEvent.BuildEvent(nameof(EPlayerMoveEnd));
        public ShareEvent EPlayerMoveToTarget = ShareEvent.BuildEvent(nameof(EPlayerMoveToTarget));
        
        private bool _isGameStart;
        private int _moveStepRemain;
        private CinemachineFramingTransposer _framingTransposer;

        public bool PlayerMoveLock => _moveStepRemain <= 0;

        private void Awake()
        {
            Init();
        }

        private void Init()
        {
            Physics2D.gravity = Vector2.zero;
            mainCamera = Camera.main;

            camBrain.m_UpdateMethod = CinemachineBrain.UpdateMethod.LateUpdate;
            _framingTransposer = virtualCam.GetCinemachineComponent<CinemachineFramingTransposer>();
            virtualCam.Follow = camTarget;
            camDragPanel.Init(this);

            EPlayerMoveStart.Subscribe(OnPlayerMoveStart);
            EPlayerMoveEnd.Subscribe(OnPlayerMoveEnd);
            EPlayerMoveToTarget.Subscribe(OnPlayerMoveToTarget);

            HidePop();

            _levels = PrGameLevelInfo.GetAll();
            _currentIndex = 0;
            JumpToLevel(_levels[_currentIndex]);
        }

        public void JumpToLevel(PrGameLevelInfo levelInfo)
        {
            if (_currentLevel) Destroy(_currentLevel.gameObject);

            _currentLevelInfo = levelInfo;
            var level = levelInfo.GetLevel();
            var go = Instantiate(level.gameObject, transform);
            _currentLevel = go.GetComponent<PrGameLevel>();
            go.SetActive(true);
            
            // data init
            // _moveStepRemain = _currentLevel.maxStepCount;
            _moveStepRemain = int.MaxValue;

            var worldToScreenMat = ScreenUtility.World2ScreenMatrix(mainCamera);
            safeZoom = worldToScreenMat.lossyScale.x * _currentLevel.ball.GetComponent<CircleCollider2D>().radius;
            
            _currentLevel.ball.Init(this);
            camDragPanel.enabled = true;
            SetCameraFollow(true);

            _isGameStart = true;
        }

        private void FixedUpdate()
        {
            UpdateCameraFollow();
        }

        private void UpdateCameraFollow()
        {
            if (_cameraFollow)
            {
                camTarget.position = _currentLevel.ball.transform.position;
            }
        }

        private void OnClickRestart()
        {
            // close ui
            HidePop();
            JumpToLevel(_currentLevelInfo);
        }

        private void OnPlayerMoveStart()
        {
            if (_moveStepRemain <= 0)
            {
                throw new Exception("Invalid Move");
            }

            // Temp: Enable camera drag before player move
            camDragPanel.enabled = false;
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

            _currentIndex++;
            _currentIndex = Mathf.Min(_currentIndex, _levels.Count - 1);
            JumpToLevel(_levels[_currentIndex]);
        }

        private void HidePop()
        {
            uiPop.SetActive(false);
        }

        public void SetPhysicsPause(bool isPause)
        {
            Time.timeScale = isPause ? 0f : 1f;
        }

        public void SetCameraFollow(bool isFollow)
        {
            _cameraFollow = isFollow;
            if (isFollow)
            {
                UpdateCameraFollow();
            }
        }

        public void DragCamera(Vector2 screenMove)
        {
            if (_cameraFollow)
            {
                throw new Exception("Set Camera Follow to false");
            }

            var offset = screenMove;
            offset *= ScreenUtility.World2ScreenMatrix(mainCamera).inverse.lossyScale;

            var camPos = camTarget.position;

            camPos.x += offset.x;
            camPos.y += offset.y;
            
            camTarget.position = camPos;
        }
    }
}
