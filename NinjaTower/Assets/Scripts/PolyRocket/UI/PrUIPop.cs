using Carotaa.Code;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace PolyRocket.UI
{
    [PageAddress("UI/PopPage")]
    public class PrUIPop: UIPage
    {
        // UI Pop
        public TMP_Text popTitle;
        public TMP_Text popBtnText;
        public Button popBtn;
        
        private class Model
        {
            public string Title;
            public string TextBtn;
            public UnityAction OnClick;
        }

        public static void Show(string title, string btnText, UnityAction onClick)
        {
            var model = new Model()
            {
                Title = title,
                TextBtn = btnText,
                OnClick = onClick,
            };
            UIManager.Instance.Push<PrUIPop>(model);
        }

        public override void OnPush()
        {
            base.OnPush();

            var model = PushParam[0] as Model;
            
            // ReSharper disable once PossibleNullReferenceException
            popTitle.text = model.Title;
            popBtnText.text = model.TextBtn;

            popBtn.onClick.AddListener(model.OnClick);

            Time.timeScale = 0f;
        }
        
        public override void OnPop()
        {
            base.OnPop();
            
            popBtn.onClick.RemoveAllListeners();
            
            Time.timeScale = 1f;
        }
    }
}