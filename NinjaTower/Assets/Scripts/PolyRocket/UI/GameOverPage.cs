using Carotaa.Code;
using PolyRocket.Game;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace PolyRocket.UI
{
    [PageAddress("UI/GameOver")]
    public class GameOverPage: UIPage
    {
        // UI Pop
        public TMP_Text popTitle;
        public TMP_Text popBtnText;
        public Button popBtn;
        
        public override void OnPush()
        {
            base.OnPush();
            
            popBtn.onClick.AddListener(OnClickRestart);

            Time.timeScale = 0f;
        }
        
        public override void OnPop()
        {
            base.OnPop();
            
            popBtn.onClick.RemoveAllListeners();
            
            Time.timeScale = 1f;
        }
        
        private void OnClickRestart()
        {
            var info = PrLevelInfo.Find(1);
            info.JumpToLevel();
            UIManager.Instance.Pop<GameOverPage>();
        }
    }
}