
using TMPro;
using UdonSharp;
using UnityEngine;
using UnityEngine.UI;
using VRC.Udon;

namespace Gems
{
    namespace UIv2
    {
        public class ButtonToggle : GemUIElement
        {

            [Header("Button")]
            [SerializeField, Tooltip("Will call <EventName> on the behaviour if button is clicked.")] UdonBehaviour script;
            [SerializeField] string eventName;

            [Header("Internal")]
            [SerializeField] UnityEngine.UI.Button btn;
            [SerializeField] Image bg;
            [SerializeField] Image border;
            [SerializeField] Image icon;
            [SerializeField] TextMeshProUGUI label;

            bool lastState; // Private only to remember the state to render in.

            void Start()
            {
                Disabled = disabled;
            }

            protected override void ApplyTheme()
            {
                if (lastState)
                {
                    bg.color = primary ? Theme.Primary : Theme.Secondary;
                    border.color = primary ? Theme.PrimaryLight : Theme.SecondaryLight;
                }
                else
                {
                    bg.color = Theme.Dark;
                    border.color = Theme.SurfaceLightest;
                }

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
                border.color = primary ? Theme.PrimaryLightest : Theme.SecondaryLightest;
                if (icon) icon.color = Theme.Light;
                if (label) label.color = Theme.Light;
            }

            public void _OnHoverExit()
            {
                if (disabled) return;

                ApplyTheme();
            }

            public void _OnClick()
            {
                if (disabled) return;
                if (script) script.SendCustomEvent(eventName);
            }

            override public void Set(bool state)
            {
                lastState = state;
                ApplyTheme();
            }
        }
    }
}