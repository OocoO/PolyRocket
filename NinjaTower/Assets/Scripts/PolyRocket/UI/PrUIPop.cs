using Carotaa.Code;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace PolyRocket.UI
{
    public class PrUIPop: PageBase
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

        public override void OnPush(object[] pushParam)
        {
            base.OnPush(pushParam);

            var module = pushParam[0] as Model;
            
            popTitle.text = "Game Over";
            popBtnText.text = "Restart";

            var onClick = new Button.ButtonClickedEvent();
            // ReSharper disable once PossibleNullReferenceException
            onClick.AddListener(module.OnClick);
            popBtn.onClick = onClick;
        }
    }
}