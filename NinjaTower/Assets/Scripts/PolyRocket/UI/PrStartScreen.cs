using Carotaa.Code;
using PolyRocket.Game;
using UnityEngine.UI;

namespace PolyRocket.UI
{
    [PageAddress("UI/StartScreen")]
    public class PrStartScreen : UIPage
    {
        public Button m_StartBtn;

        public override void OnPush()
        {
            base.OnPush();
            
            m_StartBtn.onClick.AddListener(OnClickStartBtn);
        }

        private void OnClickStartBtn()
        {
            UIManager.Instance.Pop(this);
            PrLevelManager.Instance.JumpToLevel(PrLevelInfo.Find(1));
        }
    }
}