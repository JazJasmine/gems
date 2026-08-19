
using Gems.UIv2;
using System.Xml.Linq;
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

public class ExampleRadioState : StateBase
{
    [SerializeField] RadioSelection radio;
    [SerializeField] int state;


    void Start()
    {
        Render();
    }

    public void OnIntRequest()
    {
        state = radio.State;
        Render();
    }

    void Render()
    {
        radio.Set(State);
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
