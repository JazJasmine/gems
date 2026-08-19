
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
        public class ToggleSlide : GemUIElement
        {
            [Header("Toggle")]
            [SerializeField, Tooltip("Will call <EventName> on the behaviour on click.")] UdonBehaviour script;
            [SerializeField] string eventName;

            [Header("Internal")]
            [SerializeField] UnityEngine.UI.Button btn;
            [SerializeField] Animator animator;
            [SerializeField] Image handle;
            [SerializeField] Image icon;
            [SerializeField] Image bg;
            [SerializeField] TextMeshProUGUI label;

            bool lastState; // Private only to remember the state to render in.

            void Start()
            {
                Disabled = disabled;
            }

            override protected void ApplyTheme()
            {
                if (label) label.color = Theme.Light;
                handle.color = Theme.Light;
            }

            override protected void _Disable()
            {
                btn.interactable = false;
                icon.gameObject.SetActive(false);
                if (label) label.color = Theme.SurfaceLight;
                handle.color = Theme.SurfaceLight;
            }

            override protected void _Enable()
            {
                btn.interactable = true;
                icon.gameObject.SetActive(true);
                ApplyTheme();
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
                if (label) label.color = primary ? Theme.Primary : Theme.Secondary;
            }

            public void _OnHoverExit()
            {
                if (disabled) return;
                if (!hover) return;
                if (label) label.color = Theme.Light;
            }

            public override void Set(bool state)
            {
                lastState = state;
                animator.SetBool("Toggled", state);

                // Color Application
                if (primary)
                {
                    icon.color = state ? Theme.Primary : Theme.SurfaceLightest;
                    bg.color = state ? Theme.Primary : Theme.SurfaceLightest;
                }
                else
                {
                    icon.color = state ? Theme.Secondary : Theme.SurfaceLightest;
                    bg.color = state ? Theme.Secondary : Theme.SurfaceLightest;
                }
            }
        }
    }
}