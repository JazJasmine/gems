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
        public class Dropdown : GemUIElement
        {
            [Header("Dropdown")]
            [SerializeField, Tooltip("Will call <EventName> on the behaviour")] UdonBehaviour script;
            [SerializeField] string eventName;

            [Header("Internal")]
            [SerializeField] Image dropdownBg;
            [SerializeField] Image dropdownBorder;
            [SerializeField] Image templateBg;
            [SerializeField] Image templateBorder;
            [SerializeField] TextMeshProUGUI label;
            [SerializeField] TextMeshProUGUI itemLabel;
            [SerializeField] Toggle toggleItem;
            [SerializeField] TMP_Dropdown dropdown;

            int internalState = 0;

            void Start()
            {
                Disabled = disabled;
            }

            protected override void ApplyTheme()
            {
                label.color = Theme.Light;
                itemLabel.color = Theme.Light;

                dropdownBg.color = Theme.Dark;
                templateBg.color = Theme.Dark;

                dropdownBorder.color = Theme.SurfaceLightest;
                templateBorder.color = Theme.SurfaceLightest;

                // Not exposed to Udon, need to be done aprio
                //ColorBlock block = primary ? Theme.PrimaryColorBlock() : Theme.SecondaryColorBlock();
                //block.normalColor = Theme.Surface;
                //toggleItem.colors = block;
            }

            protected override void _Disable()
            {
                dropdown.interactable = false;
                dropdownBg.color = Theme.SurfaceLight;
                dropdownBorder.color = Theme.Dark;
                label.color = Theme.Dark;
            }

            protected override void _Enable()
            {
                dropdown.interactable = true;
                ApplyTheme();
            }

            public void _OnHoverEnter()
            {
                if (disabled) return;
                if (!hover) return;

                dropdownBg.color = primary ? Theme.Primary : Theme.Secondary;
                dropdownBorder.color = primary ? Theme.PrimaryLight : Theme.SecondaryLight;
            }

            public void _OnHoverExit()
            {
                if (disabled) return;
                if (!hover) return;

                ApplyTheme();
            }

            public void _OnValueChange()
            {
                // Same idea like the RadioSelection.
                if (disabled) return;
                internalState = dropdown.value;

                if (script) script.SendCustomEvent(eventName);
            }

            override public void Set(int state)
            {
                internalState = state;

                dropdown.SetValueWithoutNotify(state);
                dropdown.RefreshShownValue();
            }

            public int State
            {
                get => internalState;
            }

            override public int Int
            {
                get => internalState;
            }
        }
    }
}