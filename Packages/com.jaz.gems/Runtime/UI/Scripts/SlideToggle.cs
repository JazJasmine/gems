
using TMPro;
using UdonSharp;
using UnityEngine;
using UnityEngine.UI;
using VRC.SDKBase;
using VRC.Udon;

namespace Gems
{
    namespace UI
    {
        [UdonBehaviourSyncMode(BehaviourSyncMode.Manual)]
        public class SlideToggle : Base
        {
            [Header("Toggle")]
            [SerializeField] bool state = false;

            [Tooltip("Will call <EventName> on the behaviour, including a state(bool) parameter. <EventName> needs to be NetworkCallable.")]
            [SerializeField] UdonBehaviour script;
            [SerializeField] string eventName;

            [Header("Internal")]
            [SerializeField] UnityEngine.UI.Button btn;
            [SerializeField] Animator animator;
            [SerializeField] Image hoverImg;
            [SerializeField] Image handle;
            [SerializeField] Image icon;
            [SerializeField] Image bg;
            [SerializeField] TextMeshProUGUI label;

            GameObject hoverObject;

            void Start()
            {
                if (hoverImg) hoverObject = hoverImg.gameObject;
                State = state;
                if (script) script.SendCustomNetworkEvent(VRC.Udon.Common.Interfaces.NetworkEventTarget.Self, eventName, State);

                Disabled = disabled;
            }

            override protected void ApplyTheme()
            {
                if (label) label.color = Theme.Light;
                handle.color = Theme.Light;
                hoverImg.color = primary ? Theme.Primary : Theme.Secondary;
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

                State = !State;
                script.SendCustomNetworkEvent(VRC.Udon.Common.Interfaces.NetworkEventTarget.Self, eventName, State);
            }

            public void _OnHoverEnter()
            {
                if (disabled) return;
                if (!hover) return;
                if (hoverObject) hoverObject.SetActive(true);
                if (label) label.color = primary ? Theme.Primary : Theme.Secondary;
            }

            public void _OnHoverExit()
            {
                if (disabled) return;
                if (!hover) return;
                if (hoverObject) hoverObject.SetActive(false);
                if (label) label.color = Theme.Light;
            }

            public bool State
            {
                get => state;
                set
                {
                    state = value;

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
}