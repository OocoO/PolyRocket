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
    public class PrGameManager : MonoSingleton<PrGameManager>
    {
        public CinemachineBrain m_camBrain;
        public CinemachineVirtualCamera m_virtualCam;
        public CinemachineConfiner2D m_camConfiner;
        public Transform m_camTarget;
        
        private List<PrGameLevelInfo> _levels;
        private PrGameLevelInfo _currentLevelInfo;
        private PrGameLevel _currentLevel;
        private int _currentIndex;

        private bool _cameraFollow;
        
        private bool _isGameStart;
        private bool _enableCameraDrag;

        private Vector2 _posLast;

        
        private void Awake()
        {
            Physics2D.gravity = Vector2.zero;

            m_camBrain.m_UpdateMethod = CinemachineBrain.UpdateMethod.LateUpdate;
            m_virtualCam.Follow = m_camTarget;

            _levels = PrGameLevelInfo.GetAll();
            _currentIndex = 0;
        }

        public void JumpToLevel(PrGameLevelInfo levelInfo)
        {
            if (_currentLevel) Destroy(_currentLevel.gameObject);

            _currentLevelInfo = levelInfo;
            var level = levelInfo.GetLevel();
            var go = Instantiate(level.gameObject, transform);
            _currentLevel = go.GetComponent<PrGameLevel>();
            go.SetActive(true);

            m_camConfiner.m_BoundingShape2D = _currentLevel.camConfine.PolygonCollider;
            m_camConfiner.InvalidateCache();

            // temp disable camera drag
            // _enableCameraDrag = true;
            
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
                m_camTarget.position = _currentLevel.ball.ballPhysics.Position;
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

        private static void SetPhysicsPause(bool isPause)
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
    }
}
