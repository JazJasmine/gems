
using Gems.UIv2;
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

public class ExampleStringState : StateBase
{
    [SerializeField] GemUIElement element;
    [SerializeField] string state;

    public void Start()
    {
        Render();
    }

    // Sadly Text Fields are a little different. They will always have their "internal" state we have to look up...
    public void OnStringChange()
    {
        state = element.Text;
        Render();
    }

    public void OnActionButton()
    {
        state = element.Text;
        Debug.Log($"String is {State}");

        Debug.Log("Action");
        // Clear after
        state = "";
        Render();
    }

    void Render()
    {
        element.Set(State);
        OnChange();
    }

    void OnChange()
    {
        Debug.Log($"String is {State}");
    }

    public string State
    {
        get => state;
    }
}
