
using UdonSharp;
using UnityEngine;
using VRC.SDK3.UdonNetworkCalling;
using VRC.SDKBase;
using VRC.Udon;

public class ExampleScript : UdonSharpBehaviour
{
    [SerializeField] Gems.UI.Base uiElement;
    [SerializeField] Gems.UI.RadioSelectionGroup uiElement1;

    [NetworkCallable]
    public void _OnExample(bool state)
    {
        uiElement.Disabled = state;
        uiElement1.Disabled = state;
    }

    [NetworkCallable]
    public void _OnExample2(string text)
    {
        Debug.Log($"HELLO {text}");
    }
}
