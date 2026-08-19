
using Gems.UIv2;
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;


// Trying to sync everything after input, could be solved differently too
[UdonBehaviourSyncMode(BehaviourSyncMode.Manual)]
public class ExampleState : StateBase
{
    [SerializeField] GemUIElement firstNameInput;
    [SerializeField] GemUIElement lastNameInput;
    [SerializeField] GemUIElement descriptionInput;
    [SerializeField] GemUIElement availableToggle;
    [SerializeField] GemUIElement muteToggle;
    [SerializeField] GemUIElement identitySelection;


    // State
    [UdonSynced] string firstName;
    [UdonSynced] string lastName;
    [UdonSynced] string description;
    [UdonSynced] bool available = true;
    [UdonSynced] bool mute;
    [UdonSynced] int identity;

    void Start()
    {
        Render();
    }

    public override void OnDeserialization() => Render();

    void Render()
    {
        firstNameInput.Set(firstName);
        lastNameInput.Set(lastName);
        descriptionInput.Set(description);
        availableToggle.Set(available);
        muteToggle.Set(mute);
        identitySelection.Set(identity);
    }

    string ToIdentity()
    {
        switch (identity)
        {
            case 0: return "Female";
            case 1: return "Male";
            case 2: return "Other";
            default:
                return "Other";
        }
    }

    public void OnFirstName()
    {
        TakeOwnershipIfNeeded();
        firstName = firstNameInput.Text;
        RequestSerialization();
        Render();
    }

    public void OnLastName()
    {
        TakeOwnershipIfNeeded();
        lastName = lastNameInput.Text;
        RequestSerialization();
        Render();
    }

    public void OnClear()
    {
        TakeOwnershipIfNeeded();
        firstName = "";
        lastName = "";
        RequestSerialization();
        Render();
    }

    public void OnDescription()
    {
        TakeOwnershipIfNeeded();
        description = descriptionInput.Text;
        RequestSerialization();
        Render();
    }

    public void OnAvailable() {
        TakeOwnershipIfNeeded();
        available = !available;
        RequestSerialization();
        Render();
    }

    public void OnMute() {
        TakeOwnershipIfNeeded();
        mute = !mute;
        RequestSerialization();
        Render();
    }

    public void OnIdentity() {
        TakeOwnershipIfNeeded();
        identity = identitySelection.Int;
        RequestSerialization();
        Render();
    }

    public void OnLog()
    {
        Debug.Log($"{firstName},{lastName},{description}, Available: {available}, Mute: {mute}, {ToIdentity()}");
    }
}
