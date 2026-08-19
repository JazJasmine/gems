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
        public class RadioSelectionElement : GemUIElement
        {
            [Header("Radio Selection Element")]
            [SerializeField] RadioSelection radioSelection;

            [Header("Internal")]
            [SerializeField] Image border;
            [SerializeField] Image handle;
            [SerializeField] TextMeshProUGUI label;
            [SerializeField] GameObject handleObject;

            bool lastState;

            void Start()
            {
                Disabled = disabled;
            }

            protected override void ApplyTheme()
            {
                if (label)
                {
                    if(primary) label.color = lastState ? Theme.Primary : Theme.Light;
                    else label.color = lastState ? Theme.Secondary : Theme.Light;
                }
                border.color = Theme.SurfaceLightest;
                handle.color = primary ? Theme.Primary : Theme.Secondary;
            }

            public void _OnClick()
            {
                if (lastState) return; // Already selected
                if (Disabled) return;

                radioSelection._OnClick(transform.GetSiblingIndex());
            }

            public void _OnHoverEnter()
            {
                if (lastState) return; // Don't hover if is selected
                if (disabled) return;
                if (!hover) return;

                if (label) label.color = primary ? Theme.Primary : Theme.Secondary;
                //border.color = primary ? Theme.Primary : Theme.Secondary;

                handleObject.SetActive(true);
            }

            public void _OnHoverExit()
            {
                if (lastState) return;
                if (disabled) return;
                if (!hover) return;

                handleObject.SetActive(false);

                ApplyTheme();
            }

            override public void Set(bool state)
            {
                if (disabled) return;
                lastState = state;

                // Color Application
                if (primary)
                {
                    if (label) label.color = lastState ? Theme.Primary : Theme.Light;
                    border.color = lastState ? Theme.Primary : Theme.SurfaceLightest;
                    handleObject.SetActive(lastState);
                }
                else
                {
                    if (label) label.color = lastState ? Theme.Secondary : Theme.Light;
                    border.color = lastState ? Theme.Secondary : Theme.SurfaceLightest;
                    handleObject.SetActive(lastState);
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
                handleObject.SetActive(lastState);
                ApplyTheme();
            }
        }
    }
}