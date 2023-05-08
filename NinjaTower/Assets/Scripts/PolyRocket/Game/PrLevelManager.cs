using System.Collections.Generic;
using Carotaa.Code;
using PolyRocket.UI;
using UnityEngine;

namespace PolyRocket.Game
{
    public class PrLevelManager : Singleton<PrLevelManager>
    {
        private bool _cameraFollow;
        private PrLevel _currentLevel;
        private PrLevelInfo _currentLevelInfo;
        private bool _enableCameraDrag;
        
        private Vector2 _posLast;
        
        public PrLevel CurrentLevel => _currentLevel;

        public void JumpToLevel(PrLevelInfo levelInfo)
        {
            if (_currentLevel)
            {
                _currentLevel.OnPop();
                Object.Destroy(_currentLevel.gameObject);
            }

            _currentLevelInfo = levelInfo;
            var level = levelInfo.GetLevel();
            var go = Object.Instantiate(level.gameObject);
            _currentLevel = go.GetComponent<PrLevel>();
            
            go.SetActive(true);
            _currentLevel.OnPush();
        }
    }
}