
using System.Linq;
using UdonSharp;
using UnityEngine;
using VRC.SDK3.Data;
using VRC.SDKBase;
using VRC.Udon;

public class RoleManager : UdonSharpBehaviour
{
    [SerializeField] Roleplayer referenceRoleplayer;
    [SerializeField] RoleplayData data;

    DataDictionary roleplayerByPlayer = new DataDictionary(); // {"jazjasmine": REFERENCE<R}
    DataDictionary roleByPlayer = new DataDictionary(); // {"jazjasmine": "officeWorker", "fiction": "unassigned"}

    private void Start()
    {
    }

    public override void OnPlayerJoined(VRCPlayerApi player)
    {
        // only Instance Owner need to keep track of this
        if (!Networking.IsInstanceOwner) return;

        roleplayerByPlayer.Add(player.displayName, GetByPlayer(player));
        roleByPlayer.Add(player.displayName, "unassigned");
    }

    public override void OnPlayerLeft(VRCPlayerApi player) 
    {
        // only Instance Owner need to keep track of this
        if (!Networking.IsInstanceOwner) return;

        roleplayerByPlayer.Remove(player.displayName);
        roleByPlayer.Remove(player.displayName); // -> might get a cooldown system
    }

    public void _AssignRole(string playerName, string roleId)
    {
        if (!data.Roles.ContainsKey(roleId)) return;
        if (roleplayerByPlayer.ContainsKey(playerName) && roleByPlayer.ContainsKey(playerName)) return;

        var role = data.Roles[roleId].DataDictionary;

        roleByPlayer[playerName] = roleId;

        // Update persons roleplay data
        ((Roleplayer)roleplayerByPlayer[playerName].Reference).SendCustomNetworkEvent(VRC.Udon.Common.Interfaces.NetworkEventTarget.Owner, "AssignRole",
            roleId, role["roleName"].String, role["description"].String, role["vibe"].String, role["behavior"].String, role["rules"].String);
    }

    public void DEBUG_ASSIGN()
    {
        _AssignRole("[0] Local Player", "intern");
    }

    Roleplayer GetByPlayer(VRCPlayerApi player)
    {
        return (Roleplayer)Networking.FindComponentInPlayerObjects(player, referenceRoleplayer);
    }
}
