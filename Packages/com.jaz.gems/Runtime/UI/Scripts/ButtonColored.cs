
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using VRC.Udon;

namespace Gems
{
    namespace UI
    {
        public class ButtonColored : Base
        {

            [Header("Button")]
            // Might add a toggle state possibility. Not now tho
            [Tooltip("Will call <EventName> on the behaviour if button is clicked. <EventName> needs to be NetworkCallable.")]
            [SerializeField] UdonBehaviour script;
            [SerializeField] string eventName;

            [Header("Internal")]
            [SerializeField] UnityEngine.UI.Button btn;
            [SerializeField] Image bg;
            [SerializeField] Image border;
            [SerializeField] Image icon;
            [SerializeField] TextMeshProUGUI label;


            protected override void ApplyTheme()
            {
                bg.color = primary ? Theme.Primary : Theme.Secondary;
                border.color = primary ? Theme.PrimaryLightest : Theme.SecondaryLightest;

                if (icon) icon.color = Theme.Light;
                if (label) label.color = Theme.Light;
            }

            override protected void _Disable()
            {
                btn.interactable = false;
                bg.color = Theme.SurfaceLight;
                border.color = Theme.Dark;
                if (icon) icon.color = Theme.Dark;
                if (label) label.color = Theme.Dark;

            }

            override protected void _Enable()
            {
                btn.interactable = true;

                ApplyTheme();
            }

            public void _OnHoverEnter()
            {
                if (disabled) return;

                bg.color = primary ? Theme.PrimaryLight : Theme.SecondaryLight;
            }

            public void _OnHoverExit()
            {
                if (disabled) return;

                ApplyTheme();
            }

            public void _OnClick()
            {
                if (disabled) return;
                if (script) script.SendCustomNetworkEvent(VRC.Udon.Common.Interfaces.NetworkEventTarget.Self, eventName);
            }
        }
    }
}