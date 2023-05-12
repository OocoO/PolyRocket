using System.Collections;
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
        
        public void JumpToLevel(PrLevelInfo levelInfo)
        {
            MonoHelper.Instance.StartCoroutine(ChangeLevel(levelInfo));
        }

        private IEnumerator ChangeLevel(PrLevelInfo next)
        {
            if (_currentLevel)
            {
                _currentLevel.OnPop();
                Object.Destroy(_currentLevel.gameObject);
                
                yield return null;
            }

            _currentLevelInfo = next;
            var level = next.GetLevel();
            var go = Object.Instantiate(level.gameObject);
            _currentLevel = go.GetComponent<PrLevel>();
            _currentLevel.OnPush();
            go.SetActive(true);
        }
    }
}