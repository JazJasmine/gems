using UdonSharp;
using UnityEngine;

namespace Gems
{
    namespace UI
    {

        // This is the Base UI that applies to every Element. It expects definition of a theme to apply on enable of UI elements
        public class Base : EmeraldBehaviour
        {
            protected override string LogName => "Gems.UI.Base";
            public GemTheme Theme;

            [Header("Common Modifier")]
            [SerializeField] protected bool disabled = false;
            [SerializeField] protected bool primary = true;
            [SerializeField] protected bool hover = true;

            virtual protected void ApplyTheme()
            {
                LogWarn($"{gameObject.name} does NOT implement ApplyTheme method");
            }

            virtual protected void _Enable()
            {
                LogWarn($"{gameObject.name} does NOT implement Enable method");
            }

            virtual protected void _Disable()
            {
                LogWarn($"{gameObject.name} does NOT implement Disable method");
            }

            private void OnEnable()
            {
                ApplyTheme();
            }

            public bool Disabled
            {
                get => disabled;
                set
                {
                    disabled = value;

                    if (disabled)
                    {
                        _Disable();
                    }
                    else
                    {
                        _Enable();
                    }
                }
            }
        }


    }
}