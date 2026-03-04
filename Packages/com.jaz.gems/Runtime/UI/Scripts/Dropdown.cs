
using System.Drawing;
using TMPro;
using UdonSharp;
using UnityEngine;
using UnityEngine.UI;
using VRC.SDKBase;
using VRC.Udon;
using VRC.SDK3.Components;

namespace Gems
{
    namespace UI
    {

        public class Dropdown : Base
        {
            [Header("Dropdown")]
            [Tooltip("Will call <EventName> on the behaviour, including a selection(int, string) parameter. <EventName> needs to be NetworkCallable.")]
            [SerializeField] UdonBehaviour script;
            [SerializeField] string eventName;

            [Header("Internal")]
            [SerializeField] UnityEngine.UI.Image dropdownBg;
            [SerializeField] UnityEngine.UI.Image dropdownBorder;
            [SerializeField] UnityEngine.UI.Image templateBg;
            [SerializeField] UnityEngine.UI.Image templateBorder;
            [SerializeField] TextMeshProUGUI label;
            [SerializeField] TextMeshProUGUI itemLabel;
            [SerializeField] Toggle toggleItem;
            [SerializeField] TMP_Dropdown dropdown;

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
                if (disabled) return;
                if(script) script.SendCustomNetworkEvent(VRC.Udon.Common.Interfaces.NetworkEventTarget.Self, eventName, dropdown.value, label.text);
            }
        }
    }
}