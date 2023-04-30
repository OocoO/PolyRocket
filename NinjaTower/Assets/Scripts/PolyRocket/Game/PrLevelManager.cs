using System.Collections.Generic;
using Carotaa.Code;
using PolyRocket.UI;
using UnityEngine;

namespace PolyRocket.Game
{
    public class PrLevelManager : Singleton<PrLevelManager>
    {
        private bool _cameraFollow;
        private int _currentIndex;
        private PrGameLevel _currentLevel;
        private PrGameLevelInfo _currentLevelInfo;
        private bool _enableCameraDrag;

        private bool _isGameStart;
        private List<PrGameLevelInfo> _levels;

        private Vector2 _posLast;


        protected override void OnCreate()
        {
            Physics2D.gravity = Vector2.zero;

            _levels = PrGameLevelInfo.GetAll();
            _currentIndex = 0;
        }

        public void JumpToLevel(PrGameLevelInfo levelInfo)
        {
            if (_currentLevel) Object.Destroy(_currentLevel.gameObject);

            _currentLevelInfo = levelInfo;
            var level = levelInfo.GetLevel();
            var go = Object.Instantiate(level.gameObject);
            _currentLevel = go.GetComponent<PrGameLevel>();
            go.SetActive(true);


            _isGameStart = true;
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

        private void OnPlayerTriggerFlag(PrActor flag)
        {
            if (_isGameStart)
            {
                // do some effect
                Object.Destroy(flag.gameObject);
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
    }
}