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
        public class RadioSelection : Base
        {
            [Header("Radio Selection")]
            [SerializeField] RadioSelectionGroup radioGroup;
            [SerializeField] bool state;

            [Header("Internal")]
            [SerializeField] Image border;
            [SerializeField] Image handle;
            [SerializeField] Image hoverImg;
            [SerializeField] TextMeshProUGUI label;
            [SerializeField] GameObject handleObject;
            [SerializeField] GameObject hoverObject;

            void Start()
            {
                Disabled = disabled;
            }

            protected override void ApplyTheme()
            {
                if (label) label.color = Theme.Light;
                border.color = Theme.SurfaceLightest;
                handle.color = primary ? Theme.Primary : Theme.Secondary;
                hoverImg.color = primary ? Theme.Primary : Theme.Secondary;
            }

            public void _OnClick()
            {
                if (state) return; // Already selected
                if (Disabled) return;

                radioGroup.State = transform.GetSiblingIndex();
            }

            public void _OnHoverEnter()
            {
                if (state) return; // Don't hover if is selected
                if (disabled) return;
                if (!hover) return;

                if (label) label.color = primary ? Theme.Primary : Theme.Secondary;
                //border.color = primary ? Theme.Primary : Theme.Secondary;

                if (hoverObject) hoverObject.SetActive(true);
                handleObject.SetActive(true);
            }

            public void _OnHoverExit()
            {
                if (state) return;
                if (disabled) return;
                if (!hover) return;

                if (hoverObject) hoverObject.SetActive(false);
                handleObject.SetActive(false);

                ApplyTheme();
            }

            void OnStateChange()
            {
                if(disabled) return;
                if (state && hoverObject) hoverObject.SetActive(false);

                // Color Application
                if (primary)
                {
                    if (label) label.color = state ? Theme.Primary : Theme.Light;
                    border.color = state ? Theme.Primary : Theme.SurfaceLightest;
                    handleObject.SetActive(state);
                }
                else
                {
                    if (label) label.color = state ? Theme.Secondary : Theme.Light;
                    border.color = state ? Theme.Secondary : Theme.SurfaceLightest;
                    handleObject.SetActive(state);
                }
            }

            override protected void _Disable()
            {
                if (label) label.color = Theme.SurfaceLight;
                handle.color = Theme.Dark;
                border.color = Theme.Dark;
            }

            override protected void _Enable()
            {
                handleObject.SetActive(State);
                ApplyTheme();
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