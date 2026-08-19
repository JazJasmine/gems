
using UdonSharp;
using UnityEngine;
using UnityEngine.UI;
using VRC.SDKBase;
using VRC.Udon;

namespace Gems
{
    namespace UIv2
    {
        // This is the Base UI that applies to every Element. It expects definition of a theme to apply on enable of UI elements
        public class GemUIElement : EmeraldBehaviour
        {
            protected override string LogName => "Gems.UIv2.Base";
            public Theme Theme;

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
                if (Theme == null) return;
                ApplyTheme();
            }

            virtual public void Set(string state)
            {
                LogWarn($"{gameObject.name} does NOT implement a Set[string] method");
            }

            virtual public void Set(bool state)
            {
                LogWarn($"{gameObject.name} does NOT implement a Set[bool] method");
            }

            virtual public void Set(int state)
            {
                LogWarn($"{gameObject.name} does NOT implement a Set[int] method");
            }

            virtual public string Text
            {
                get
                {
                    LogWarn($"{gameObject.name} does NOT implement a Text method");
                    return "";
                }
            }

            virtual public int Int
            {
                get
                {
                    LogWarn($"{gameObject.name} does NOT implement a Int method");
                    return -1;
                }
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