using System;
using System.Collections.Generic;
using Carotaa.Code;
using Cinemachine;
using PolyRocket.UI;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using Object = UnityEngine.Object;

namespace PolyRocket.Game
{
    // dependency: PrGlobal <- GameScripts <- Launcher
    public class PrGameLauncher : MonoBehaviour
    {
        // UIHud
        public CinemachineBrain camBrain;
        public CinemachineVirtualCamera virtualCam;
        public CinemachineConfiner2D camConfiner;
        public EmptyTouchPanel camDragPanel;
        public Transform camTarget;
        
        private List<PrGameLevelInfo> _levels;
        private PrGameLevelInfo _currentLevelInfo;
        private PrGameLevel _currentLevel;
        private int _currentIndex;

        private bool _cameraFollow;
        
        private bool _isGameStart;
        private bool _enableCameraDrag;
        private CinemachineFramingTransposer _framingTransposer;

        private Vector2 _posLast;
        private PrGlobal _global;

        
        private void Awake()
        {
            Init();
        }

        private void Init()
        {
            Physics2D.gravity = Vector2.zero;

            camBrain.m_UpdateMethod = CinemachineBrain.UpdateMethod.LateUpdate;
            _framingTransposer = virtualCam.GetCinemachineComponent<CinemachineFramingTransposer>();
            virtualCam.Follow = camTarget;
            camDragPanel.Init(this);

            _global = new PrGlobal();
            _global.MainCamera = Camera.main;
            _global.EPlayerTriggerTrap += OnPlayerTriggerTrap;
            _global.EPlayerMoveTriggerFlag += OnPlayerTriggerFlag;
            
            // InitUI
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
            
            var worldToScreenMat = ScreenUtility.World2ScreenMatrix(_global.MainCamera);
            _global.BallScreenRadius = worldToScreenMat.lossyScale.x * _currentLevel.ball.col.radius;

            camConfiner.m_BoundingShape2D = _currentLevel.camConfine.PolygonCollider;
            camConfiner.InvalidateCache();

            // temp disable camera drag
            // _enableCameraDrag = true;
            
            _currentLevel.ball.Init(_global);
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

        private void OnPlayerTriggerTrap()
        {
            LevelFailed();
        }

        private void OnPlayerTriggerFlag(PrActorBase flag)
        {
            if (_isGameStart)
            {
                // do some effect
                Destroy(flag.gameObject);
                _currentLevel.FlagCount--;
                if (_currentLevel.FlagCount <= 0)
                {
                    _isGameStart = false;
                    ShowGameSuccess();
                }
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
            PrUIPop.Show("Game Over", "Restart", OnClickRestart);
            
            SetPhysicsPause(true);
        }

        private void ShowGameSuccess()
        {
            PrUIPop.Show("Level Complete", "Next", OnClickGotoNext);
            
            SetPhysicsPause(true);
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
            UIManager.Instance.Pop<PrUIPop>();
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
            offset *= ScreenUtility.World2ScreenMatrix(_global.MainCamera).inverse.lossyScale;

            var camPos = camTarget.position;

            camPos.x += offset.x;
            camPos.y += offset.y;
            
            camTarget.position = camPos;
        }


        // Camera drag
        public void OnPointerDown(PointerEventData eventData)
        {
            _posLast = eventData.position;
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            _enableCameraDrag = false;
            
            _currentLevel.ball.OnPointerClick(eventData);
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
