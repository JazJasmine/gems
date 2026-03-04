
using UdonSharp;
using UnityEngine;
using VRC.SDK3.Data;
using VRC.SDK3.UdonNetworkCalling;
using VRC.SDKBase;
using VRC.Udon;

[UdonBehaviourSyncMode(BehaviourSyncMode.Manual)]
public class Roleplayer : UdonSharpBehaviour
{
    VRCPlayerApi owner;

    // Roleplay Data - not synced for now, as in just instance owner and the owning player needs to know, which is achieved with network calls.
    /*[UdonSynced]*/ string roleId;
    /*[UdonSynced]*/ string roleName;
    /*[UdonSynced]*/ string description;
    /*[UdonSynced]*/ string vibe;
    /*[UdonSynced]*/ string behavior;
    /*[UdonSynced]*/ string rules;

    // Internals
    DataList activeTasks = new DataList();
    DataList recentlyDone = new DataList();


    void Start()
    {
        owner = Networking.GetOwner(gameObject);
    }

    [NetworkCallable]
    public void AssignRole(string id, string name, string d, string v, string b, string r)
    {
        roleId= id;
        roleName = name;
        description = d;
        vibe = v;
        behavior = b; 
        rules =r;

        // Take initial set of tasks...
    }
}
