
using TMPro;
using UdonSharp;
using UnityEngine;
using UnityEngine.UI;
using VRC.SDKBase;
using VRC.Udon;

namespace Gems
{
    namespace UIv2
    {
        public class ToggleBox : GemUIElement
        {
            [Header("Toggle")]
            [SerializeField, Tooltip("Will call <EventName> on the behaviour on click.")] UdonBehaviour script;
            [SerializeField] string eventName;

            [Header("Internal")]
            [SerializeField] Image border;
            [SerializeField] Image bg;
            [SerializeField] Image icon;
            [SerializeField] TextMeshProUGUI label;

            bool lastState; // Private only to remember the state to render in.

            void Start()
            {
                Disabled = disabled;
            }

            override protected void ApplyTheme()
            {
                if (label) label.color = Theme.Light;
                border.color = Theme.SurfaceLightest;
                bg.color = primary ? Theme.Primary : Theme.Secondary;
                if (icon) icon.color = primary ? Theme.PrimaryLightest : Theme.SecondaryLightest;
            }

            override protected void _Disable()
            {
                if (label) label.color = Theme.SurfaceLight;
                border.color = Theme.SurfaceLight;
                bg.color = Theme.Dark;
                if (icon) icon.color = Theme.SurfaceLight;
            }

            override protected void _Enable()
            {
                ApplyTheme();
                OnStateChange();
            }

            public void _OnClick()
            {
                if (disabled) return;
                if (script) script.SendCustomEvent(eventName);
            }

            public void _OnHoverEnter()
            {
                if (disabled) return;
                if (!hover) return;
                border.color = primary ? Theme.Primary : Theme.Secondary;
                if (label) label.color = primary ? Theme.Primary : Theme.Secondary;
            }

            public void _OnHoverExit()
            {
                if (disabled) return;
                if (!hover) return;
                ApplyTheme();
                OnStateChange();
            }

            void OnStateChange()
            {
                if (lastState)
                {
                    border.color = primary ? Theme.PrimaryLightest : Theme.SecondaryLightest;
                }
                else
                {
                    border.color = Theme.SurfaceLightest;
                }

                bg.gameObject.SetActive(lastState);
            }

            public override void Set(bool state)
            {
                lastState = state;
                ApplyTheme();
                OnStateChange();
            }
        }
    }
}