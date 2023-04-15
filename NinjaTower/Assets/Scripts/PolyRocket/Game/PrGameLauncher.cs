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
        public CinemachineConfiner2D camConfiner;
        public EmptyTouchPanel camDragPanel;
        public Transform camTarget;

        public int maxShotForce;
        public float speedDecrease;
        public float ballScreenRadius; // the size of the ball: screen coord

        private List<PrGameLevelInfo> _levels;
        private PrGameLevelInfo _currentLevelInfo;
        private PrGameLevel _currentLevel;
        private int _currentIndex;

        private bool _cameraFollow;
        
        public Camera mainCamera;

        public ShareEvent EPlayerMoveStart = ShareEvent.BuildEvent(nameof(EPlayerMoveStart));
        public ShareEvent EPlayerMoveToTarget = ShareEvent.BuildEvent(nameof(EPlayerMoveToTarget));
        
        private bool _isGameStart;
        private bool _enableCameraDrag;
        private CinemachineFramingTransposer _framingTransposer;

        private Vector2 _posLast;

        
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
            
            var worldToScreenMat = ScreenUtility.World2ScreenMatrix(mainCamera);
            ballScreenRadius = worldToScreenMat.lossyScale.x * _currentLevel.ball.col.radius;

            camConfiner.m_BoundingShape2D = _currentLevel.camConfine.PolygonCollider;
            camConfiner.InvalidateCache();

            // temp disable camera drag
            // _enableCameraDrag = true;
            
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
                camTarget.position = _currentLevel.ball.ballPhysics.Position;
            }
        }

        private void OnClickRestart()
        {
            // close ui
            HidePop();
            JumpToLevel(_currentLevelInfo);
        }

        public void OnPlayerTriggerTrap()
        {
            LevelFailed();
        }

        private void OnPlayerMoveToTarget()
        {
            if (_isGameStart)
            {
                _isGameStart = false;
                ShowGameSuccess();
            }
        }

        private void LevelFailed()
        {
            // game over: failed
            _isGameStart = false;
            ShowGameOver();
        }

        private void ShowGameOver()
        {
            popTitle.text = "Game Over";
            popBtnText.text = "Restart";

            var onClick = new Button.ButtonClickedEvent();
            onClick.AddListener(OnClickRestart);
            popBtn.onClick = onClick;
            
            SetPhysicsPause(true);
            uiPop.SetActive(true);
        }

        private void ShowGameSuccess()
        {
            popTitle.text = "Level Complete";
            popBtnText.text = "Next";

            var onClick = new Button.ButtonClickedEvent();
            onClick.AddListener(OnClickGotoNext);
            popBtn.onClick = onClick;
            
            SetPhysicsPause(true);
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
            SetPhysicsPause(false);
            uiPop.SetActive(false);
        }

        private void SetPhysicsPause(bool isPause)
        {
            Time.timeScale = isPause ? 0f : 1f;
        }

        private void SetCameraFollow(bool isFollow)
        {
            _cameraFollow = isFollow;
            if (isFollow)
            {
                UpdateCameraFollow();
            }
        }

        private void DragCamera(Vector2 screenMove)
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

        private void BlowWind(PointerEventData eventData)
        {
            _enableCameraDrag = false;
            
            _currentLevel.ball.OnWindBlow(eventData);
        }
        
        
        // Camera drag
        public void OnPointerDown(PointerEventData eventData)
        {
            _posLast = eventData.position;
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            BlowWind(eventData);
        }
        public void OnBeginDrag(PointerEventData eventData)
        {
            if (!_enableCameraDrag) return;
            
            SetCameraFollow(false);
            CancelInvoke(nameof(ResetCameraFollow));
        }

        public void OnDrag(PointerEventData eventData)
        {
            if (!_enableCameraDrag) return;
            
            var dMove = _posLast - eventData.position;
            _posLast = eventData.position;
            
            DragCamera(dMove);
        }

        public void OnEndDrag(PointerEventData eventData)
        {
            Invoke(nameof(ResetCameraFollow),1f);
        }
        
        private void ResetCameraFollow()
        {
            SetCameraFollow(true);
        }
    }
}
