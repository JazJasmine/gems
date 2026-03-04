
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
        public class BoxToggle : Base
        {
            [Header("Toggle")]
            [SerializeField] bool state = false;

            [Tooltip("Will call <EventName> on the behaviour, including a state(bool) parameter. <EventName> needs to be NetworkCallable.")]
            [SerializeField] UdonBehaviour script;
            [SerializeField] string eventName;

            [Header("Internal")]
            [SerializeField] Image border;
            [SerializeField] Image bg;
            [SerializeField] Image icon;
            [SerializeField] TextMeshProUGUI label;


            void Start()
            {
                State = state;
                if (script) script.SendCustomNetworkEvent(VRC.Udon.Common.Interfaces.NetworkEventTarget.Self, eventName, State);

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

                State = !State;
                if (script) script.SendCustomNetworkEvent(VRC.Udon.Common.Interfaces.NetworkEventTarget.Self, eventName, State);
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
                if (state)
                {
                    border.color = primary ? Theme.PrimaryLightest : Theme.SecondaryLightest;
                }
                else
                {
                    border.color = Theme.SurfaceLightest;
                }
                bg.gameObject.SetActive(state);
            }

            public bool State
            {
                get => state;
                set
                {
                    state = value;
                    OnStateChange();
                }
            }
        }
    }
}