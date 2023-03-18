using System;
using UnityEngine;

namespace PolyRocket.Game
{
    public class PrGameLauncher : MonoBehaviour
    {
        public PrGameLevel levelOne;
        public PrGameLevel levelTwo;

        public static PrGameLauncher Instance;

        private PrGameLevel _currentLevel;
        
        public Camera mainCamera;

        private void Awake()
        {
            Instance = this;
            
            Init();
        }

        private void Init()
        {
            Physics2D.gravity = Vector2.zero;
            mainCamera = Camera.main;
            
            levelOne.gameObject.SetActive(false);
            levelTwo.gameObject.SetActive(false);
            
            JumpToLevel(levelOne);
        }

        public void JumpToLevel(PrGameLevel level)
        {
            if (_currentLevel) _currentLevel.gameObject.SetActive(false);
            
            level.gameObject.SetActive(true);
            level.ball.Init(this);
            _currentLevel = level;
        }
    }
}
