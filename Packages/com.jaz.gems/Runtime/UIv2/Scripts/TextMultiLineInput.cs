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
        public class TextMultiLineInput : GemUIElement
        {
            [Header("Text Input - Multiline")]
            [SerializeField] bool hasActionButton = false;
            [SerializeField, Tooltip("Will call <EventName> on the behaviour on action click")] UdonBehaviour actionScript;
            [SerializeField] string actionEventName;

            [SerializeField, Tooltip("Will call <EventName> on the behaviour on value changed")] UdonBehaviour script;
            [SerializeField] string eventName;

            [Header("Big Text Input")]
            [SerializeField] int characterLimit;

            [Header("Internal")]
            [SerializeField] TMP_InputField inputField;
            [SerializeField] TextMeshProUGUI inputText;
            [SerializeField] TextMeshProUGUI label;
            [SerializeField] Image bg;
            [SerializeField] Image btnBg;
            [SerializeField] Image btnIcon;
            [SerializeField] Image border;
            [SerializeField] Image seperator;
            [SerializeField] TextMeshProUGUI placeholder;
            [SerializeField] GameObject actionButton;
            [SerializeField] TextMeshProUGUI characterLeftUi;

            void Start()
            {
                Disabled = disabled;
                //inputField.characterLimit = characterLimit;
            }

            protected override void ApplyTheme()
            {
                actionButton.SetActive(hasActionButton);

                if (label) label.color = Theme.Light;
                inputText.color = Theme.Light;
                bg.color = Theme.Dark;
                placeholder.color = Theme.SurfaceLightest;
                border.color = Theme.SurfaceLightest;
                seperator.color = Theme.SurfaceLightest;

                btnBg.color = Theme.SurfaceLight;
                btnIcon.color = Theme.Light;

                // Apparently this is not supported for TMP InputFields :(
                //if (primary)
                //{
                //    inputField.selectionColor = new Color(Theme.Primary.r, Theme.Primary.g, Theme.Primary.b, .6f);
                //} else
                //{
                //    inputField.selectionColor = new Color(Theme.Secondary.r, Theme.Secondary.g, Theme.Secondary.b, .6f);
                //}
            }

            override protected void _Disable()
            {
                if (label) label.color = Theme.SurfaceLight;
                inputField.interactable = false;
                inputText.color = Theme.Dark;

                bg.color = Theme.SurfaceLight;
                border.color = Theme.Dark;
                seperator.color = Theme.Dark;
                placeholder.color = Theme.Dark;

                btnBg.color = Theme.Dark;
                btnIcon.color = Theme.SurfaceLight;
            }

            override protected void _Enable()
            {
                inputField.interactable = true;

                ApplyTheme();
            }

            public void _OnValueChanged()
            {
                if (script) script.SendCustomEvent(eventName);
            }

            public void _OnActionClick()
            {
                if (disabled) return;
                if (hasActionButton && actionScript) actionScript.SendCustomEvent(actionEventName);
            }

            public void _OnHoverEnter()
            {
                if (disabled) return;
                if (!hover) return;

                if (label) label.color = primary ? Theme.Primary : Theme.Secondary;
                bg.color = primary ? Theme.Primary : Theme.Secondary;
                border.color = primary ? Theme.PrimaryLight : Theme.SecondaryLight;
                seperator.color = primary ? Theme.PrimaryLight : Theme.SecondaryLight;

                btnBg.color = Theme.Light;
                btnIcon.color = primary ? Theme.Primary : Theme.Secondary;

                placeholder.color = Theme.Light;
            }

            public void _OnHoverExit()
            {
                if (disabled) return;
                if (!hover) return;

                ApplyTheme();
            }

            public void _OnHoverButtonEnter()
            {
                if (disabled) return;

                btnBg.color = primary ? Theme.Primary : Theme.Secondary;
                btnIcon.color = Theme.Light;
            }

            public void _OnHoverButtonExit()
            {
                if (disabled) return;

                btnBg.color = Theme.SurfaceLight;
                btnIcon.color = Theme.Light;
            }

            public override void Set(string state)
            {
                inputField.text = state;

                var leftCharacter = characterLimit - inputField.text.Length;
                characterLeftUi.text = leftCharacter.ToString();
            }

            override public string Text
            {
                get => inputField.text;
            }
        }
    }
}