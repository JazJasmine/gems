
using Gems.UIv2;
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

[UdonBehaviourSyncMode(BehaviourSyncMode.Manual)]
public class ExampleBoolSyncState : StateBase
{
    [SerializeField] GemUIElement element;
    [SerializeField, UdonSynced] bool state;

    public void Start()
    {
        Render();
    }

    // This one is called by the UI element
    public void RequestToggle()
    {
        TakeOwnershipIfNeeded();
        state = !state;
        RequestSerialization();
        Render();
    }

    public override void OnDeserialization() => Render();

    public void RequestToggle(bool s)
    {
        state = s;
        Render();
    }

    void Render()
    {
        element.Set(State);
        OnToggle();
    }

    void OnToggle()
    {
        Debug.Log($"Toggle is {State}");
    }

    public bool State
    {
        get => state;
        set
        {
            RequestToggle(value);
        }
    }
}
