
using Gems.UIv2;
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

public class ExampleDropdownState : StateBase
{
    [SerializeField] Dropdown dropdown;
    [SerializeField] int state;

    void Start()
    {
        Render();
    }

    public void OnIntRequest()
    {
        state = dropdown.State;
        Render();
    }

    void Render()
    {
        dropdown.Set(State);
        OnChange();
    }

    void OnChange()
    {
        Debug.Log($"Int is {State}");
    }

    public int State
    {
        get => state;
    }
}