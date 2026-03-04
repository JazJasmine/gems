
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

namespace Gems
{
    namespace UI
    {
        [UdonBehaviourSyncMode(BehaviourSyncMode.Manual)]
        public class RadioSelectionGroup : UdonSharpBehaviour
        {
            [Header("Common Modifier")]
            [SerializeField] protected bool disabled = false;

            [Header("Radio Selection Group")]
            [SerializeField] RadioSelection[] selections;
            [SerializeField] int state = 0;

            [Tooltip("Will call <EventName> on the behaviour if selection changes, including a state(int) parameter. <EventName> needs to be NetworkCallable.")]
            [SerializeField] UdonBehaviour script;
            [SerializeField] string eventName;

            void Start()
            {
                State = state;
            }

            void _Disable()
            {
                foreach (var selection in selections)
                {
                    selection.Disabled = true;
                }
            }

            void _Enable()
            {
                foreach (var selection in selections)
                {
                    selection.Disabled = false;
                }
            }

            public int State
            {
                get => state;
                set
                {
                    state = value;
                    for (int i = 0; i < selections.Length; i++)
                    {
                        var selection = selections[i];
                        selection.State = i == state;
                    }

                    if (script != null) script.SendCustomNetworkEvent(VRC.Udon.Common.Interfaces.NetworkEventTarget.Self, eventName, state);
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