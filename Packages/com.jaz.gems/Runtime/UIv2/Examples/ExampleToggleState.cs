// Example on how to use the UI elements

using Gems.UIv2;
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;


public class ExampleToggleState : StateBase
{
    [SerializeField] GemUIElement element;
    [SerializeField] bool state;

    public void Start()
    {
        Render();
    }

    // This one is called by the UI element
    public void RequestToggle()
    {
        state = !state;
        Render();
    }

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