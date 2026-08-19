
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

namespace Gems
{
    namespace UIv2
    {
        public class RadioSelection : GemUIElement
        {
            [Header("Radio Selection")]
            [SerializeField] RadioSelectionElement[] selections;

            [SerializeField, Tooltip("Will call <EventName> on the behaviour if selection changes.")] UdonBehaviour script;
            [SerializeField] string eventName;

            // Sadly this will have to keep a "internal" state, just like text input...as I try to avoid usind SendCustomNetworkEvent.
            int internalState = 0;

            void Start()
            {
                Disabled = disabled;
            }

            protected override void ApplyTheme()
            {
                
            }

            public void _OnClick(int state)
            {
                // RadioSelection.internalState -sendCustomEvent-> StateClass.actualState -Set-> RadioSelection.internalState (again)
                // I could use a SendCustomNetworkEvent and send the parameter and skip the whole internalState, however then it needs to use NetworkCallable,
                // which then assumes every subsequent script to be using a synced mode. With this I can keep UI elements unsynced
                internalState = state;
                if (script) script.SendCustomEvent(eventName);
            }

            override protected void _Disable()
            {
                foreach (var selection in selections)
                {
                    selection.Disabled = true;
                }
            }

            override protected void _Enable()
            {
                foreach (var selection in selections)
                {
                    selection.Disabled = false;
                }
            }

            public override void Set(int state)
            {
                internalState = state;
                for (int i = 0; i < selections.Length; i++)
                {
                    var selection = selections[i];
                    selection.Set(i == state);
                }
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

